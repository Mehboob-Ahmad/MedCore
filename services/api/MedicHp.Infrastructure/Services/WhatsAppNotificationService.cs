using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MedicHp.Infrastructure.Services;

public class WhatsAppNotificationService : IWhatsAppNotificationService
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IGenericRepository<Appointment> _appointmentRepo;
    private readonly WhatsAppTemplateSettings _templates;
    private readonly ILogger<WhatsAppNotificationService> _logger;

    public WhatsAppNotificationService(
        IWhatsAppService whatsAppService,
        IGenericRepository<Appointment> appointmentRepo,
        IOptions<WhatsAppTemplateSettings> templates,
        ILogger<WhatsAppNotificationService> logger)
    {
        _whatsAppService = whatsAppService;
        _appointmentRepo = appointmentRepo;
        _templates = templates.Value;
        _logger = logger;
    }

    private async Task<Appointment?> GetAppointmentWithDetailsAsync(Guid appointmentId, CancellationToken ct)
    {
        return await _appointmentRepo.FirstOrDefaultAsync(
            a => a.Id == appointmentId,
            q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
                     Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(q, a => a.Patient), 
                     a => a.Doctor),
            cancellationToken: ct);
    }

    private string GetPatientName(Appointment appt) => $"{appt.Patient.FirstName} {appt.Patient.LastName}".Trim();
    private string GetDoctorName(Appointment appt) => $"Dr. {appt.Doctor.LastName}".Trim();
    private string GetDateStr(Appointment appt) => appt.ScheduledAt.ToString("dd MMMM yyyy");
    private string GetTimeStr(Appointment appt) => appt.ScheduledAt.ToString("hh:mm tt");

    public async Task<string?> SendPaymentReminderAsync(Guid appointmentId, decimal amount, CancellationToken ct = default)
    {
        var appt = await GetAppointmentWithDetailsAsync(appointmentId, ct);
        if (appt == null) return null;

        var components = new object[]
        {
            new
            {
                type = "body",
                parameters = new object[]
                {
                    new { type = "text", parameter_name = "patient_name", text = GetPatientName(appt) },
                    new { type = "text", parameter_name = "doctor_name", text = GetDoctorName(appt) },
                    new { type = "text", parameter_name = "payment_amount", text = amount.ToString("G29") },
                    new { type = "text", parameter_name = "date", text = GetDateStr(appt) }
                }
            }
        };

        _logger.LogInformation("Attempting to send {TemplateName} for Appointment {AppointmentId}", _templates.PaymentReminder, appointmentId);
        return await _whatsAppService.SendTemplateMessageAsync(appt.Patient.PhoneNumber, _templates.PaymentReminder, "en", components, ct);
    }

    public async Task<string?> SendPaymentSuccessAsync(Guid appointmentId, decimal amount, CancellationToken ct = default)
    {
        var appt = await GetAppointmentWithDetailsAsync(appointmentId, ct);
        if (appt == null) return null;

        var components = new object[]
        {
            new
            {
                type = "body",
                parameters = new object[]
                {
                    new { type = "text", parameter_name = "patient_name", text = GetPatientName(appt) },
                    new { type = "text", parameter_name = "payment_amount", text = amount.ToString("G29") },
                    new { type = "text", parameter_name = "doctor_name", text = GetDoctorName(appt) }
                }
            }
        };

        _logger.LogInformation("Attempting to send {TemplateName} for Appointment {AppointmentId}", _templates.PaymentSuccess, appointmentId);
        return await _whatsAppService.SendTemplateMessageAsync(appt.Patient.PhoneNumber, _templates.PaymentSuccess, "en", components, ct);
    }

    public async Task<string?> SendPaymentOverdueAsync(Guid appointmentId, decimal amount, CancellationToken ct = default)
    {
        var appt = await GetAppointmentWithDetailsAsync(appointmentId, ct);
        if (appt == null) return null;

        var components = new object[]
        {
            new
            {
                type = "body",
                parameters = new object[]
                {
                    new { type = "text", parameter_name = "patient_name", text = GetPatientName(appt) },
                    new { type = "text", parameter_name = "payment_amount", text = amount.ToString("G29") },
                    new { type = "text", parameter_name = "doctor_name", text = GetDoctorName(appt) }
                }
            }
        };

        _logger.LogInformation("Attempting to send {TemplateName} for Appointment {AppointmentId}", _templates.PaymentOverdue, appointmentId);
        return await _whatsAppService.SendTemplateMessageAsync(appt.Patient.PhoneNumber, _templates.PaymentOverdue, "en", components, ct);
    }

    public async Task<string?> SendAppointmentReminderAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await GetAppointmentWithDetailsAsync(appointmentId, ct);
        if (appt == null) return null;

        var components = new object[]
        {
            new
            {
                type = "body",
                parameters = new object[]
                {
                    new { type = "text", parameter_name = "doctor_name", text = GetDoctorName(appt) },
                    new { type = "text", parameter_name = "date", text = GetDateStr(appt) },
                    new { type = "text", parameter_name = "time", text = GetTimeStr(appt) }
                }
            }
        };

        _logger.LogInformation("Attempting to send {TemplateName} for Appointment {AppointmentId}", _templates.AppointmentReminder, appointmentId);
        return await _whatsAppService.SendTemplateMessageAsync(appt.Patient.PhoneNumber, _templates.AppointmentReminder, "en", components, ct);
    }

    public async Task<string?> SendAppointmentConfirmationAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await GetAppointmentWithDetailsAsync(appointmentId, ct);
        if (appt == null) return null;

        var components = new object[]
        {
            new
            {
                type = "body",
                parameters = new object[]
                {
                    new { type = "text", parameter_name = "patient_name", text = GetPatientName(appt) },
                    new { type = "text", parameter_name = "doctor_name", text = GetDoctorName(appt) },
                    new { type = "text", parameter_name = "date", text = GetDateStr(appt) },
                    new { type = "text", parameter_name = "time", text = GetTimeStr(appt) }
                }
            }
        };

        _logger.LogInformation("Attempting to send {TemplateName} for Appointment {AppointmentId}", _templates.AppointmentConfirmation, appointmentId);
        return await _whatsAppService.SendTemplateMessageAsync(appt.Patient.PhoneNumber, _templates.AppointmentConfirmation, "en", components, ct);
    }
}
