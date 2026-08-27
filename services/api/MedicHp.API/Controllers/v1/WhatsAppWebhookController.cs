using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MedicHp.Application.Common;
using MedicHp.Application.Features.WhatsApp;
using MedicHp.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MedicHp.API.Controllers.v1;

/// <summary>
/// Webhook receiver for Meta WhatsApp Cloud API.
/// This endpoint is called directly by Meta, so it bypasses JWT authentication.
/// </summary>
[ApiController]
[Route("api/whatsapp/webhook")]
[AllowAnonymous]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IWhatsAppEventQueue _queue;
    private readonly WhatsAppSettings _settings;
    private readonly ILogger<WhatsAppWebhookController> _logger;

    public WhatsAppWebhookController(
        IWhatsAppEventQueue queue,
        IOptions<WhatsAppSettings> settings,
        ILogger<WhatsAppWebhookController> logger)
    {
        _queue = queue;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Webhook Verification (GET).
    /// Meta calls this to verify the webhook URL when you configure it in the App Dashboard.
    /// </summary>
    [HttpGet]
    public IActionResult VerifyWebhook(
        [FromQuery(Name = "hub.mode")] string? mode,
        [FromQuery(Name = "hub.verify_token")] string? token,
        [FromQuery(Name = "hub.challenge")] string? challenge)
    {
        if (mode == "subscribe" && token == _settings.VerifyToken)
        {
            _logger.LogInformation("WhatsApp webhook verified successfully.");
            // Must return the challenge exactly as a plain string
            return Content(challenge ?? string.Empty, "text/plain");
        }

        _logger.LogWarning("WhatsApp webhook verification failed. Token mismatch.");
        return Forbid();
    }

    /// <summary>
    /// Webhook Payload Receiver (POST).
    /// Meta posts event payloads (messages, status updates) here.
    /// MUST return 200 OK immediately to prevent Meta from retrying.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook()
    {
        // 1. Verify signature if AppSecret is configured
        if (!string.IsNullOrEmpty(_settings.AppSecret))
        {
            if (!Request.Headers.TryGetValue("x-hub-signature-256", out var signatureHeader))
            {
                _logger.LogWarning("Missing x-hub-signature-256 header.");
                return BadRequest("Missing signature");
            }

            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            Request.Body.Position = 0; // Reset for JSON model binding

            var expectedSignature = "sha256=" + ComputeHmacSha256(rawBody, _settings.AppSecret);
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedSignature),
                    Encoding.UTF8.GetBytes(signatureHeader!)))
            {
                _logger.LogWarning("Invalid webhook signature.");
                return BadRequest("Invalid signature");
            }
        }

        // 2. Deserialize payload
        try
        {
            var payload = await JsonSerializer.DeserializeAsync<WhatsAppWebhookPayload>(
                Request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null || payload.Object != "whatsapp_business_account")
            {
                return NotFound();
            }

            // 3. Enqueue for background processing (do NOT await processing)
            await _queue.EnqueueAsync(payload);

            // 4. Return 200 OK immediately
            return Ok("EVENT_RECEIVED");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse WhatsApp webhook payload.");
            return BadRequest();
        }
    }

    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
