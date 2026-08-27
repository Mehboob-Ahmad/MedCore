using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Enums;

namespace MedicHp.Domain.Entities.Messaging;

/// <summary>
/// Represents a WhatsApp message record for audit and tracking purposes.
/// Stores only metadata — never stores message body, medical information,
/// access tokens, passwords, OTPs, or other sensitive patient data.
/// </summary>
public class WhatsAppMessage : AuditableEntity
{
    /// <summary>
    /// Meta's unique message identifier (wamid.*)
    /// Used for idempotency — duplicate webhook deliveries are ignored.
    /// </summary>
    public string WhatsAppMessageId { get; set; } = null!;

    /// <summary>
    /// Phone number in E.164 format (e.g., +923001234567).
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Optional FK to MedicHp User if the phone number matches an existing user.
    /// </summary>
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// The type of WhatsApp message (text, image, document, etc.).
    /// </summary>
    public WhatsAppMessageType MessageType { get; set; }

    /// <summary>
    /// Whether this message was incoming (from patient) or outgoing (from MedicHp).
    /// </summary>
    public WhatsAppMessageDirection Direction { get; set; }

    /// <summary>
    /// Current delivery status of the message.
    /// </summary>
    public WhatsAppMessageStatus Status { get; set; }

    /// <summary>
    /// Timestamp of the last status change from Meta.
    /// </summary>
    public DateTime? StatusTimestamp { get; set; }

    /// <summary>
    /// Original message timestamp from Meta.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Meta error code if the message delivery failed.
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Meta error description if the message delivery failed.
    /// Must never contain sensitive information.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Optional JSON metadata for non-sensitive context only.
    /// Must NEVER contain: message body, medical info, tokens, passwords, OTPs.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// FK to the doctor's WhatsApp connection that sent or received this message.
    /// Used for multi-tenant routing of incoming webhooks.
    /// </summary>
    public Guid? DoctorProfileId { get; set; }
    public DoctorProfile? DoctorProfile { get; set; }

    public string RecipientPhoneNumber { get; set; } = null!;
}
