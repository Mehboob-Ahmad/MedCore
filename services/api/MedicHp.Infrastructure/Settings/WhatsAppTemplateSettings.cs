namespace MedicHp.Infrastructure.Settings;

/// <summary>
/// Configuration for Meta WhatsApp Cloud API template names.
/// Bound from configuration section "WhatsAppTemplates".
/// </summary>
public class WhatsAppTemplateSettings
{
    public string PaymentReminder { get; set; } = "payment_reminder";
    public string PaymentSuccess { get; set; } = "payment_success";
    public string PaymentOverdue { get; set; } = "payment_overdue";
    public string AppointmentReminder { get; set; } = "appointment_reminder";
    public string AppointmentConfirmation { get; set; } = "appointment_confirmation";
}
