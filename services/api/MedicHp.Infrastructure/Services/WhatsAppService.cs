using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MedicHp.Application.Common;
using MedicHp.Application.Features.WhatsApp;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Messaging;
using MedicHp.Domain.Enums;
using MedicHp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MedicHp.Infrastructure.Services;

/// <summary>
/// WhatsApp Cloud API service — Global Sender.
///
/// Sending flow:
///   WhatsAppSettings.GlobalAccessToken + PhoneNumberId → Meta Graph API
///
/// Webhook processing flow:
///   Persist WhatsAppMessage using PhoneNumber
/// </summary>
public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly IGenericRepository<DoctorProfile> _doctorRepo;
    private readonly IGenericRepository<WhatsAppMessage> _messageRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly WhatsAppSettings _settings;
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(
        HttpClient httpClient,
        IGenericRepository<DoctorProfile> doctorRepo,
        IGenericRepository<WhatsAppMessage> messageRepo,
        IGenericRepository<User> userRepo,
        IUnitOfWork unitOfWork,
        IOptions<WhatsAppSettings> settings,
        ILogger<WhatsAppService> logger)
    {
        _httpClient = httpClient;
        _doctorRepo = doctorRepo;
        _messageRepo = messageRepo;
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
        _logger = logger;
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
        var normalized = phoneNumber.Trim().Replace("+", "");
        if (normalized.StartsWith("0"))
        {
            normalized = "92" + normalized.Substring(1);
        }
        return normalized;
    }

    /// <inheritdoc />
    public async Task<string?> SendTextMessageAsync(string recipientPhoneNumber, string text, CancellationToken ct = default)
    {
        var normalizedPhone = NormalizePhoneNumber(recipientPhoneNumber);
        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = normalizedPhone,
            type = "text",
            text = new { preview_url = false, body = text }
        };

        return await SendToMetaAsync(payload, normalizedPhone, WhatsAppMessageType.Text, ct);
    }

    /// <inheritdoc />
    public async Task<string?> SendTemplateMessageAsync(string recipientPhoneNumber, string templateName, string languageCode, object[]? components = null, CancellationToken ct = default)
    {
        var normalizedPhone = NormalizePhoneNumber(recipientPhoneNumber);
        var template = new Dictionary<string, object>
        {
            ["name"] = templateName,
            ["language"] = new { code = languageCode }
        };

        if (components != null)
            template["components"] = components;

        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = normalizedPhone,
            type = "template",
            template
        };

        return await SendToMetaAsync(payload, normalizedPhone, WhatsAppMessageType.Template, ct);
    }

    /// <inheritdoc />
    public async Task ProcessWebhookAsync(WhatsAppWebhookPayload payload, CancellationToken ct = default)
    {
        if (payload.Entry == null) return;

        foreach (var entry in payload.Entry)
        {
            if (entry.Changes == null) continue;

            foreach (var change in entry.Changes)
            {
                if (change.Field != "messages") continue;

                // Process incoming messages
                if (change.Value.Messages != null)
                {
                    foreach (var msg in change.Value.Messages)
                    {
                        await ProcessIncomingMessageAsync(msg, ct);
                    }
                }

                // Process status updates
                if (change.Value.Statuses != null)
                {
                    foreach (var status in change.Value.Statuses)
                    {
                        await ProcessStatusUpdateAsync(status, ct);
                    }
                }
            }
        }
    }

    // ========================================================================
    // Private helpers
    // ========================================================================

    /// <summary>
    /// Send a message to Meta Cloud API through the Global MedicHp connection.
    /// </summary>
    private async Task<string?> SendToMetaAsync(
        object payload,
        string recipientPhoneNumber,
        WhatsAppMessageType messageType,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_settings.GlobalAccessToken) || string.IsNullOrEmpty(_settings.PhoneNumberId))
        {
            _logger.LogError("WhatsApp GlobalAccessToken or PhoneNumberId is not configured.");
            return null;
        }

        try
        {
            var url = $"https://graph.facebook.com/{_settings.ApiVersion}/{_settings.PhoneNumberId}/messages";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.GlobalAccessToken);
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            string? wamid = null;

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("messages", out var messages)
                    && messages.GetArrayLength() > 0)
                {
                    wamid = messages[0].GetProperty("id").GetString();
                }

                _logger.LogInformation(
                    "WhatsApp message sent to {RecipientPhone}. MessageId: {WamId}",
                    recipientPhoneNumber, wamid);
            }
            else
            {
                _logger.LogError(
                    "WhatsApp API error. Status: {StatusCode}. Response: {ResponseBody}",
                    response.StatusCode, responseBody);
            }

            var messageRecord = new WhatsAppMessage
            {
                WhatsAppMessageId = wamid ?? $"pending_{Guid.NewGuid()}",
                PhoneNumber = recipientPhoneNumber, // User's phone number
                RecipientPhoneNumber = recipientPhoneNumber, // The phone number the message was sent to
                MessageType = messageType,
                Direction = WhatsAppMessageDirection.Outgoing,
                Status = response.IsSuccessStatusCode ? WhatsAppMessageStatus.Sent : WhatsAppMessageStatus.Failed,
                Timestamp = DateTime.UtcNow,
                ErrorCode = response.IsSuccessStatusCode ? null : (int)response.StatusCode,
                ErrorMessage = response.IsSuccessStatusCode ? null : TruncateForSafety(responseBody)
            };

            // Link to doctor if recipient matches a doctor's WhatsApp number
            var doctor = await _doctorRepo.FirstOrDefaultAsync(
                d => d.WhatsAppNumber == recipientPhoneNumber,
                cancellationToken: ct);
            messageRecord.DoctorProfileId = doctor?.Id;

            var user = await _userRepo.FirstOrDefaultAsync(
                u => u.PhoneNumber == recipientPhoneNumber,
                cancellationToken: ct);
            messageRecord.UserId = user?.Id;

            await _messageRepo.AddAsync(messageRecord, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return wamid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send WhatsApp message to {RecipientPhone}", recipientPhoneNumber);
            return null;
        }
    }

    private async Task ProcessIncomingMessageAsync(
        WhatsAppIncomingMessage msg,
        CancellationToken ct)
    {
        var existing = await _messageRepo.FirstOrDefaultAsync(
            m => m.WhatsAppMessageId == msg.Id,
            cancellationToken: ct);

        if (existing != null)
        {
            _logger.LogDebug("Duplicate webhook for WhatsAppMessageId {MessageId} — skipping", msg.Id);
            return;
        }

        var messageType = ParseMessageType(msg.Type);
        var timestamp = ParseUnixTimestamp(msg.Timestamp);

        var messageRecord = new WhatsAppMessage
        {
            WhatsAppMessageId = msg.Id,
            PhoneNumber = msg.From, // Sender's phone number
            RecipientPhoneNumber = _settings.PhoneNumberId, // Sent to MedicHp
            MessageType = messageType,
            Direction = WhatsAppMessageDirection.Incoming,
            Status = WhatsAppMessageStatus.Delivered,
            Timestamp = timestamp
        };

        var doctor = await _doctorRepo.FirstOrDefaultAsync(
            d => d.WhatsAppNumber == msg.From,
            cancellationToken: ct);
        messageRecord.DoctorProfileId = doctor?.Id;

        var user = await _userRepo.FirstOrDefaultAsync(
            u => u.PhoneNumber == msg.From,
            cancellationToken: ct);
        messageRecord.UserId = user?.Id;

        await _messageRepo.AddAsync(messageRecord, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Incoming WhatsApp message {MessageId} from {Phone}", msg.Id, msg.From);
    }

    private async Task ProcessStatusUpdateAsync(
        WhatsAppStatusUpdate status,
        CancellationToken ct)
    {
        var existing = await _messageRepo.FirstOrDefaultAsync(
            m => m.WhatsAppMessageId == status.Id,
            cancellationToken: ct);

        if (existing == null)
        {
            _logger.LogDebug(
                "Status update for unknown WhatsAppMessageId {MessageId} — creating placeholder record",
                status.Id);

            var placeholder = new WhatsAppMessage
            {
                WhatsAppMessageId = status.Id,
                PhoneNumber = status.RecipientId,
                RecipientPhoneNumber = status.RecipientId,
                MessageType = WhatsAppMessageType.Unknown,
                Direction = WhatsAppMessageDirection.Outgoing,
                Status = ParseMessageStatus(status.Status),
                StatusTimestamp = ParseUnixTimestamp(status.Timestamp),
                Timestamp = ParseUnixTimestamp(status.Timestamp)
            };

            var doctor = await _doctorRepo.FirstOrDefaultAsync(
                d => d.WhatsAppNumber == status.RecipientId,
                cancellationToken: ct);
            placeholder.DoctorProfileId = doctor?.Id;

            if (status.Errors?.Count > 0)
            {
                placeholder.ErrorCode = status.Errors[0].Code;
                placeholder.ErrorMessage = TruncateForSafety(status.Errors[0].Title);
            }

            await _messageRepo.AddAsync(placeholder, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return;
        }

        var newStatus = ParseMessageStatus(status.Status);
        if (newStatus > existing.Status)
        {
            existing.Status = newStatus;
            existing.StatusTimestamp = ParseUnixTimestamp(status.Timestamp);

            if (status.Errors?.Count > 0)
            {
                existing.ErrorCode = status.Errors[0].Code;
                existing.ErrorMessage = TruncateForSafety(status.Errors[0].Title);
            }

            await _messageRepo.UpdateAsync(existing, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("WhatsApp status updated: {MessageId} → {NewStatus}", status.Id, newStatus);
        }
    }

    private static WhatsAppMessageType ParseMessageType(string type) => type?.ToLowerInvariant() switch
    {
        "text" => WhatsAppMessageType.Text,
        "image" => WhatsAppMessageType.Image,
        "document" => WhatsAppMessageType.Document,
        "audio" => WhatsAppMessageType.Audio,
        "video" => WhatsAppMessageType.Video,
        "location" => WhatsAppMessageType.Location,
        "contacts" => WhatsAppMessageType.Contact,
        "sticker" => WhatsAppMessageType.Sticker,
        "interactive" => WhatsAppMessageType.Interactive,
        "button" => WhatsAppMessageType.Button,
        "template" => WhatsAppMessageType.Template,
        "reaction" => WhatsAppMessageType.Reaction,
        _ => WhatsAppMessageType.Unknown
    };

    private static WhatsAppMessageStatus ParseMessageStatus(string status) => status?.ToLowerInvariant() switch
    {
        "sent" => WhatsAppMessageStatus.Sent,
        "delivered" => WhatsAppMessageStatus.Delivered,
        "read" => WhatsAppMessageStatus.Read,
        "failed" => WhatsAppMessageStatus.Failed,
        _ => WhatsAppMessageStatus.Pending
    };

    private static DateTime ParseUnixTimestamp(string timestamp)
    {
        if (long.TryParse(timestamp, out var unix))
            return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
        return DateTime.UtcNow;
    }

    private static string? TruncateForSafety(string? text, int maxLength = 500)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Length > maxLength ? text[..maxLength] : text;
    }
}
