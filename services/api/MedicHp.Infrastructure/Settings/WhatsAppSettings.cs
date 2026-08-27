namespace MedicHp.Infrastructure.Settings;

/// <summary>
/// Platform-level WhatsApp Cloud API settings.
/// Bound from configuration section "WhatsApp" using .NET hierarchical convention.
///
/// Environment variables:
///   WhatsApp__VerifyToken    — Token for Meta webhook verification handshake
///   WhatsApp__ApiVersion     — Meta Graph API version (e.g., "v21.0")
///   WhatsApp__AppSecret      — Meta App Secret for webhook payload signature validation
///
/// NOTE: Per-doctor access tokens and phone number IDs are stored in
/// DoctorWhatsAppConnection, NOT here. This class only holds platform-level config.
/// </summary>
public class WhatsAppSettings
{
    /// <summary>
    /// Token used to verify Meta's webhook subscription challenge.
    /// Must match the value configured in the Meta App Dashboard.
    /// </summary>
    public string VerifyToken { get; set; } = string.Empty;

    /// <summary>
    /// Meta Graph API version. Configurable so it can be updated when Meta
    /// deprecates older versions without code changes.
    /// </summary>
    public string ApiVersion { get; set; } = "v21.0";

    /// <summary>
    /// Meta App Secret for HMAC-SHA256 webhook signature validation.
    /// Used to verify that incoming webhooks genuinely originate from Meta.
    /// </summary>
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// The global MedicHp WhatsApp Business Account Access Token.
    /// </summary>
    public string GlobalAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The global MedicHp WhatsApp Phone Number ID.
    /// </summary>
    public string PhoneNumberId { get; set; } = string.Empty;

    /// <summary>
    /// The global MedicHp WhatsApp Business Account ID.
    /// </summary>
    public string WabaId { get; set; } = string.Empty;
}
