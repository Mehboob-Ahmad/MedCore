using System.Text.Json.Serialization;

namespace MedicHp.Application.Features.WhatsApp;

// ============================================================================
// Meta WhatsApp Cloud API Webhook Payload DTOs
// Reference: https://developers.facebook.com/docs/whatsapp/cloud-api/webhooks
//
// Webhook structure:
//   WhatsAppWebhookPayload
//     └─ Entry[]
//          └─ Change[]
//               └─ Value
//                    ├─ Metadata (contains phone_number_id for multi-tenant routing)
//                    ├─ Messages[] (incoming messages)
//                    ├─ Statuses[] (delivery/read status updates)
//                    └─ Contacts[] (sender contact info)
// ============================================================================

/// <summary>
/// Root webhook payload from Meta WhatsApp Cloud API.
/// </summary>
public class WhatsAppWebhookPayload
{
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("entry")]
    public List<WhatsAppWebhookEntry> Entry { get; set; } = new();
}

public class WhatsAppWebhookEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("changes")]
    public List<WhatsAppWebhookChange> Changes { get; set; } = new();
}

public class WhatsAppWebhookChange
{
    [JsonPropertyName("value")]
    public WhatsAppWebhookValue Value { get; set; } = new();

    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
}

public class WhatsAppWebhookValue
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public WhatsAppMetadata Metadata { get; set; } = new();

    [JsonPropertyName("contacts")]
    public List<WhatsAppWebhookContact>? Contacts { get; set; }

    [JsonPropertyName("messages")]
    public List<WhatsAppIncomingMessage>? Messages { get; set; }

    [JsonPropertyName("statuses")]
    public List<WhatsAppStatusUpdate>? Statuses { get; set; }

    [JsonPropertyName("errors")]
    public List<WhatsAppWebhookError>? Errors { get; set; }
}

/// <summary>
/// Metadata identifying which WhatsApp Business phone number received the event.
/// phone_number_id is the key for multi-tenant routing to DoctorWhatsAppConnection.
/// </summary>
public class WhatsAppMetadata
{
    [JsonPropertyName("display_phone_number")]
    public string DisplayPhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("phone_number_id")]
    public string PhoneNumberId { get; set; } = string.Empty;
}

public class WhatsAppWebhookContact
{
    [JsonPropertyName("profile")]
    public WhatsAppContactProfile? Profile { get; set; }

    [JsonPropertyName("wa_id")]
    public string WaId { get; set; } = string.Empty;
}

public class WhatsAppContactProfile
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Incoming message from a WhatsApp user.
/// NOTE: We store only metadata (type, id, timestamp, from) — never the message body.
/// </summary>
public class WhatsAppIncomingMessage
{
    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public WhatsAppTextContent? Text { get; set; }

    [JsonPropertyName("image")]
    public WhatsAppMediaContent? Image { get; set; }

    [JsonPropertyName("document")]
    public WhatsAppMediaContent? Document { get; set; }

    [JsonPropertyName("audio")]
    public WhatsAppMediaContent? Audio { get; set; }

    [JsonPropertyName("video")]
    public WhatsAppMediaContent? Video { get; set; }

    [JsonPropertyName("location")]
    public WhatsAppLocationContent? Location { get; set; }

    [JsonPropertyName("contacts")]
    public List<WhatsAppContactContent>? Contacts { get; set; }

    [JsonPropertyName("sticker")]
    public WhatsAppMediaContent? Sticker { get; set; }

    [JsonPropertyName("interactive")]
    public WhatsAppInteractiveContent? Interactive { get; set; }

    [JsonPropertyName("button")]
    public WhatsAppButtonContent? Button { get; set; }

    [JsonPropertyName("context")]
    public WhatsAppMessageContext? Context { get; set; }

    [JsonPropertyName("errors")]
    public List<WhatsAppWebhookError>? Errors { get; set; }
}

// Content type DTOs — used for deserialization but content is NOT persisted.
public class WhatsAppTextContent
{
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}

public class WhatsAppMediaContent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }
}

public class WhatsAppLocationContent
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

public class WhatsAppContactContent
{
    [JsonPropertyName("name")]
    public WhatsAppContactName? Name { get; set; }
}

public class WhatsAppContactName
{
    [JsonPropertyName("formatted_name")]
    public string FormattedName { get; set; } = string.Empty;
}

public class WhatsAppInteractiveContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class WhatsAppButtonContent
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("payload")]
    public string? Payload { get; set; }
}

public class WhatsAppMessageContext
{
    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

/// <summary>
/// Delivery/read status update for an outgoing message.
/// </summary>
public class WhatsAppStatusUpdate
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("recipient_id")]
    public string RecipientId { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public List<WhatsAppWebhookError>? Errors { get; set; }

    [JsonPropertyName("conversation")]
    public WhatsAppConversationInfo? Conversation { get; set; }

    [JsonPropertyName("pricing")]
    public WhatsAppPricingInfo? Pricing { get; set; }
}

public class WhatsAppConversationInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("origin")]
    public WhatsAppConversationOrigin? Origin { get; set; }
}

public class WhatsAppConversationOrigin
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class WhatsAppPricingInfo
{
    [JsonPropertyName("billable")]
    public bool Billable { get; set; }

    [JsonPropertyName("pricing_model")]
    public string PricingModel { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
}

public class WhatsAppWebhookError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("error_data")]
    public WhatsAppErrorData? ErrorData { get; set; }
}

public class WhatsAppErrorData
{
    [JsonPropertyName("details")]
    public string? Details { get; set; }
}

// ============================================================================
// Response DTOs — for doctor-facing API endpoints
// ============================================================================

/// <summary>
/// Response DTO for doctor WhatsApp connection status.
/// NEVER includes access tokens or encrypted credentials.
/// </summary>
public class WhatsAppConnectionDto
{
    public Guid Id { get; set; }
    public string PhoneNumberId { get; set; } = string.Empty;
    public string DisplayPhoneNumber { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string ConnectionStatus { get; set; } = string.Empty;
    public DateTime? ConnectedAt { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
}

/// <summary>
/// Minimal status check DTO.
/// </summary>
public class WhatsAppStatusDto
{
    public bool IsConnected { get; set; }
    public string ConnectionStatus { get; set; } = string.Empty;
    public string? DisplayPhoneNumber { get; set; }
    public string? BusinessName { get; set; }
}

/// <summary>
/// Health check response for the WhatsApp subsystem.
/// NEVER exposes secrets.
/// </summary>
public class WhatsAppHealthDto
{
    public bool WebhookConfigured { get; set; }
    public string ApiVersion { get; set; } = string.Empty;
    public int QueueDepth { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
