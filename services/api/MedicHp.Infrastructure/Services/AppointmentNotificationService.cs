using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Enums;
using MedicHp.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MedicHp.Infrastructure.Services;

public class AppointmentNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NotificationSchedulerSettings _settings;
    private readonly ILogger<AppointmentNotificationService> _logger;

    public AppointmentNotificationService(
        IServiceProvider serviceProvider,
        IOptions<NotificationSchedulerSettings> settings,
        ILogger<AppointmentNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AppointmentNotificationService started with interval {Interval} minutes.", _settings.IntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AppointmentNotificationService.");
            }

            await Task.Delay(TimeSpan.FromMinutes(_settings.IntervalMinutes), stoppingToken);
        }
    }

    private async Task ProcessNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var appointmentRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Appointment>>();
        var whatsappNotificationService = scope.ServiceProvider.GetRequiredService<IWhatsAppNotificationService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        var reminderThreshold = now.AddHours(_settings.ReminderHoursBeforeAppointment);

        // 1. Appointment Reminders (24 hours before)
        var appointmentsToRemind = await appointmentRepo.GetQueryable()
            .Where(a => a.Status == "Confirmed" && 
                        a.AppointmentReminderSentAt == null && 
                        a.ScheduledAt > now && 
                        a.ScheduledAt <= reminderThreshold)
            .ToListAsync(cancellationToken);

        foreach (var appt in appointmentsToRemind)
        {
            try
            {
                await whatsappNotificationService.SendAppointmentReminderAsync(appt.Id);
                appt.AppointmentReminderSentAt = now;
                await appointmentRepo.UpdateAsync(appt, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send appointment reminder for appointment {AppointmentId}", appt.Id);
            }
        }

        // 2. Payment Reminders (24 hours before)
        var paymentsToRemind = await appointmentRepo.GetQueryable()
            .Where(a => a.Status == "Confirmed" && 
                        a.PaymentStatus == PaymentStatus.Pending.ToString() && 
                        a.PaymentReminderSentAt == null && 
                        a.ScheduledAt > now && 
                        a.ScheduledAt <= reminderThreshold)
            .ToListAsync(cancellationToken);

        foreach (var appt in paymentsToRemind)
        {
            try
            {
                // In a real app we might calculate the amount based on consultation fee minus discount etc.
                // Here we just pass 0 or a fixed amount if not available on appointment, 
                // but WhatsAppNotificationService fetches the fee from DoctorProfile anyway.
                // Passing a placeholder amount as it is fetched inside the service.
                decimal amount = appt.Doctor?.DoctorProfile?.ConsultationFee ?? 0m;
                
                await whatsappNotificationService.SendPaymentReminderAsync(appt.Id, amount);
                appt.PaymentReminderSentAt = now;
                await appointmentRepo.UpdateAsync(appt, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment reminder for appointment {AppointmentId}", appt.Id);
            }
        }

        // 3. Overdue Detection (ScheduledAt < now and PaymentStatus == Pending)
        // Send it daily until resolved. We check if it hasn't been notified today.
        var todayStart = now.Date;
        var overduePayments = await appointmentRepo.GetQueryable()
            .Where(a => a.Status == "Confirmed" && 
                        a.PaymentStatus == PaymentStatus.Pending.ToString() && 
                        a.ScheduledAt <= now &&
                        (a.PaymentOverdueNotifiedAt == null || a.PaymentOverdueNotifiedAt < todayStart))
            .ToListAsync(cancellationToken);

        foreach (var appt in overduePayments)
        {
            try
            {
                appt.PaymentStatus = PaymentStatus.Overdue.ToString();
                
                decimal amount = appt.Doctor?.DoctorProfile?.ConsultationFee ?? 0m;
                await whatsappNotificationService.SendPaymentOverdueAsync(appt.Id, amount);
                
                appt.PaymentOverdueNotifiedAt = now;
                await appointmentRepo.UpdateAsync(appt, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment overdue for appointment {AppointmentId}", appt.Id);
            }
        }

        if (appointmentsToRemind.Any() || paymentsToRemind.Any() || overduePayments.Any())
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Processed {ApptReminders} appointment reminders, {PayReminders} payment reminders, {Overdue} overdue payments.", 
                appointmentsToRemind.Count, paymentsToRemind.Count, overduePayments.Count);
        }
    }
}
