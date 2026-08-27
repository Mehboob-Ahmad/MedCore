using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Application.Features.WhatsApp;
using MedicHp.Infrastructure.Services;
using MedicHp.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MedicHp.API.Controllers.v1;

/// <summary>
/// Health check and diagnostic endpoints for the WhatsApp integration.
/// NEVER exposes secrets.
/// </summary>
[ApiController]
[Route("api/whatsapp/health")]
public class WhatsAppHealthController : ControllerBase
{
    private readonly IWhatsAppEventQueue _queue;
    private readonly WhatsAppSettings _settings;

    public WhatsAppHealthController(
        IWhatsAppEventQueue queue,
        IOptions<WhatsAppSettings> settings)
    {
        _queue = queue;
        _settings = settings.Value;
    }

    /// <summary>
    /// Public platform health check.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<WhatsAppHealthDto> GetHealth()
    {
        // Safe cast since we know the concrete implementation for metrics
        var queueDepth = (_queue as WhatsAppEventQueue)?.Count ?? 0;

        return Ok(new WhatsAppHealthDto
        {
            WebhookConfigured = !string.IsNullOrEmpty(_settings.VerifyToken),
            ApiVersion = _settings.ApiVersion,
            QueueDepth = queueDepth,
            CheckedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Authenticated test send. Only works in Dev/Staging.
    /// </summary>
    [HttpPost("test-send")]
    [Authorize]
    public async Task<ActionResult> TestSend(
        [FromQuery] string recipientPhone,
        [FromQuery] string templateType,
        [FromServices] IWhatsAppService whatsappService,
        [FromServices] IOptions<WhatsAppTemplateSettings> templateSettingsOpt,
        [FromServices] ICurrentUserService currentUserService,
        [FromServices] IWebHostEnvironment env)
    {
        if (env.IsProduction())
            return Forbid("Test send is disabled in production.");

        var userId = currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var templates = templateSettingsOpt.Value;
        object[] components = null;
        string templateName = null;

        // Dummy Test Values
        var patientName = "Mehboob";
        var doctorName = "Dr. Ahmed";
        var amount = "2500";
        var date = "25 August 2026";
        var time = "5:00 PM";

        switch (templateType.ToLowerInvariant())
        {
            case "payment_reminder":
                templateName = templates.PaymentReminder;
                components = new object[] { new { type = "body", parameters = new object[] { new { type = "text", parameter_name = "patient_name", text = patientName }, new { type = "text", parameter_name = "doctor_name", text = doctorName }, new { type = "text", parameter_name = "payment_amount", text = amount }, new { type = "text", parameter_name = "date", text = date } } } };
                break;
            case "payment_success":
                templateName = templates.PaymentSuccess;
                components = new object[] { new { type = "body", parameters = new object[] { new { type = "text", parameter_name = "patient_name", text = patientName }, new { type = "text", parameter_name = "payment_amount", text = amount }, new { type = "text", parameter_name = "doctor_name", text = doctorName } } } };
                break;
            case "payment_overdue":
                templateName = templates.PaymentOverdue;
                components = new object[] { new { type = "body", parameters = new object[] { new { type = "text", parameter_name = "patient_name", text = patientName }, new { type = "text", parameter_name = "payment_amount", text = amount }, new { type = "text", parameter_name = "doctor_name", text = doctorName } } } };
                break;
            case "appointment_reminder":
                templateName = templates.AppointmentReminder;
                components = new object[] { new { type = "body", parameters = new object[] { new { type = "text", parameter_name = "doctor_name", text = doctorName }, new { type = "text", parameter_name = "date", text = date }, new { type = "text", parameter_name = "time", text = time } } } };
                break;
            case "appointment_confirmation":
                templateName = templates.AppointmentConfirmation;
                components = new object[] { new { type = "body", parameters = new object[] { new { type = "text", parameter_name = "patient_name", text = patientName }, new { type = "text", parameter_name = "doctor_name", text = doctorName }, new { type = "text", parameter_name = "date", text = date }, new { type = "text", parameter_name = "time", text = time } } } };
                break;
            default:
                return BadRequest("Invalid templateType. Valid options: payment_reminder, payment_success, payment_overdue, appointment_reminder, appointment_confirmation");
        }

        var wamid = await whatsappService.SendTemplateMessageAsync(
            recipientPhone, 
            templateName,
            "en",
            components);

        if (wamid == null)
            return BadRequest("Failed to send message. Check logs.");

        return Ok(new { MessageId = wamid });
    }
}
