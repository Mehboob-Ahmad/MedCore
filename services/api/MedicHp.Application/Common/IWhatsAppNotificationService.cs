using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Common;

/// <summary>
/// Business-level WhatsApp notification service.
/// Provides methods to send specific templates mapped to application workflows.
/// </summary>
public interface IWhatsAppNotificationService
{
    Task<string?> SendPaymentReminderAsync(Guid appointmentId, decimal amount, CancellationToken ct = default);
    Task<string?> SendPaymentSuccessAsync(Guid appointmentId, decimal amount, CancellationToken ct = default);
    Task<string?> SendPaymentOverdueAsync(Guid appointmentId, decimal amount, CancellationToken ct = default);
    Task<string?> SendAppointmentReminderAsync(Guid appointmentId, CancellationToken ct = default);
    Task<string?> SendAppointmentConfirmationAsync(Guid appointmentId, CancellationToken ct = default);
}
