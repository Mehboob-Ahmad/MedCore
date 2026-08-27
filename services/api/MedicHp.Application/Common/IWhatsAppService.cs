using MedicHp.Application.Features.WhatsApp;

namespace MedicHp.Application.Common;

/// <summary>
/// Service interface for WhatsApp Cloud API operations.
/// All sending uses the global MedicHp WhatsApp Business Account.
/// </summary>
public interface IWhatsAppService
{
    /// <summary>
    /// Send a text message through the MedicHp WhatsApp Business number.
    /// </summary>
    /// <param name="recipientPhoneNumber">Recipient phone in E.164 format.</param>
    /// <param name="text">Message text content.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Meta's wamid message ID, or null if sending failed.</returns>
    Task<string?> SendTextMessageAsync(string recipientPhoneNumber, string text, CancellationToken ct = default);

    /// <summary>
    /// Send a template message through the MedicHp WhatsApp Business number.
    /// Phase 1 supports: appointment_confirmation, appointment_reminder,
    /// appointment_cancellation, followup_notification, fee_notification.
    /// </summary>
    /// <param name="recipientPhoneNumber">Recipient phone in E.164 format.</param>
    /// <param name="templateName">Meta-approved template name.</param>
    /// <param name="languageCode">Template language code (e.g., "en_US").</param>
    /// <param name="components">Optional template components (header, body, button params).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Meta's wamid message ID, or null if sending failed.</returns>
    Task<string?> SendTemplateMessageAsync(string recipientPhoneNumber, string templateName, string languageCode, object[]? components = null, CancellationToken ct = default);

    /// <summary>
    /// Process a raw webhook payload from Meta. Called by the background worker.
    /// </summary>
    Task ProcessWebhookAsync(WhatsAppWebhookPayload payload, CancellationToken ct = default);
}
