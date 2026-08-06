using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MedCore.Domain.Entities.Core;
using MedCore.Shared.Exceptions;
using MediatR;

namespace MedCore.Application.Features.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, bool>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<AppointmentStatusHistory> _statusHistoryRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleAppointmentCommandHandler(
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<AppointmentStatusHistory> statusHistoryRepository,
        IGenericRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _doctorProfileRepository = doctorProfileRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment == null)
            throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // Authorization: only the patient or doctor involved can reschedule
        if (appointment.PatientId != request.UserId && appointment.DoctorId != request.UserId)
            throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // Business rule: only Pending or Confirmed can be rescheduled
        if (appointment.Status is not ("Pending" or "Confirmed"))
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Status",
                    $"Appointment with status '{appointment.Status}' cannot be rescheduled.")
            });

        // Get doctor's slot duration
        var doctor = await _doctorProfileRepository.FirstOrDefaultAsync(
            d => d.UserId == appointment.DoctorId, null, cancellationToken);
        var durationMinutes = doctor?.SlotDurationMinutes ?? 30;

        // Parse new time
        var newStartTime = TimeOnly.Parse(request.NewStartTime);
        var newScheduledAt = request.NewScheduledDate.Date.Add(newStartTime.ToTimeSpan());
        var newEndTime = newScheduledAt.AddMinutes(durationMinutes);

        // Prevent past-date booking
        if (newScheduledAt <= DateTime.UtcNow)
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("NewScheduledDate", "Cannot reschedule to a past date.")
            });

        // Check for conflicts at the new time
        var now = DateTime.UtcNow;
        var conflicts = await _appointmentRepository.GetAsync(
            a => a.DoctorId == appointment.DoctorId &&
                 a.Id != appointment.Id &&
                 a.ScheduledAt < newEndTime &&
                 a.ScheduledAt.AddMinutes(a.DurationMinutes) > newScheduledAt &&
                 a.Status != "Cancelled" && a.Status != "Rejected" &&
                 (a.Status != "Reserved" || (a.ExpiresAt != null && a.ExpiresAt > now)),
            null, cancellationToken);

        if (conflicts.Any())
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("NewStartTime", "The new time slot is already booked.")
            });

        var previousStatus = appointment.Status;
        var previousScheduledAt = appointment.ScheduledAt;

        appointment.ScheduledAt = newScheduledAt;
        appointment.DurationMinutes = durationMinutes;
        appointment.Status = "Rescheduled";

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        // Record status history
        var statusHistory = new AppointmentStatusHistory
        {
            AppointmentId = appointment.Id,
            FromStatus = previousStatus,
            ToStatus = "Rescheduled",
            ChangedByUserId = request.UserId,
            Reason = request.Reason ?? $"Rescheduled from {previousScheduledAt:MMM dd, yyyy HH:mm} to {newScheduledAt:MMM dd, yyyy HH:mm}."
        };
        await _statusHistoryRepository.AddAsync(statusHistory, cancellationToken);

        // Notify the other party
        var notifyUserId = appointment.PatientId == request.UserId
            ? appointment.DoctorId
            : appointment.PatientId;

        var notification = new Notification
        {
            UserId = notifyUserId,
            Type = "AppointmentRescheduled",
            Title = "Appointment Rescheduled",
            Body = $"An appointment has been rescheduled to {newScheduledAt:MMM dd, yyyy} at {newScheduledAt:HH:mm}.",
            ReferenceType = "Appointment",
            ReferenceId = appointment.Id,
            SentAt = DateTime.UtcNow
        };
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
