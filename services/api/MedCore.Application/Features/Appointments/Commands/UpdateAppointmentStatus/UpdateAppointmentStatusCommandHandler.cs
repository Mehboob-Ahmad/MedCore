using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Shared.Exceptions;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MedCore.Domain.Entities.Core;
using MediatR;

namespace MedCore.Application.Features.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, bool>
{
    private static readonly Dictionary<string, HashSet<string>> ValidTransitions = new()
    {
        { "Pending", new HashSet<string> { "Confirmed", "Rejected" } },
        { "Confirmed", new HashSet<string> { "Completed", "Cancelled", "NoShow", "Rescheduled" } },
        { "Rescheduled", new HashSet<string> { "Confirmed", "Rejected", "Cancelled" } }
    };

    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<AppointmentStatusHistory> _statusHistoryRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentStatusCommandHandler(
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<AppointmentStatusHistory> statusHistoryRepository,
        IGenericRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        
        if (appointment == null || appointment.DoctorId != request.DoctorId)
            throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // Validate status transition
        if (!ValidTransitions.TryGetValue(appointment.Status, out var allowedStatuses) ||
            !allowedStatuses.Contains(request.Status))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Status",
                    $"Cannot transition from '{appointment.Status}' to '{request.Status}'.")
            });
        }

        var previousStatus = appointment.Status;
        appointment.Status = request.Status;

        // Handle status-specific logic
        if (request.Status == "Rejected" && request.SuggestedNewTime.HasValue)
        {
            // Doctor is suggesting an alternative time instead of outright rejecting
            appointment.SuggestedNewTime = request.SuggestedNewTime.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.DoctorNotes))
        {
            appointment.DoctorNotes = request.DoctorNotes;
        }

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        // Record status history
        var statusHistory = new AppointmentStatusHistory
        {
            AppointmentId = appointment.Id,
            FromStatus = previousStatus,
            ToStatus = request.Status,
            ChangedByUserId = request.DoctorId,
            Reason = request.Reason
        };
        await _statusHistoryRepository.AddAsync(statusHistory, cancellationToken);

        // Notify patient
        var (title, body) = GetNotificationContent(request.Status, appointment.ScheduledAt);
        var notification = new Notification
        {
            UserId = appointment.PatientId,
            Type = $"Appointment{request.Status}",
            Title = title,
            Body = body,
            ReferenceType = "Appointment",
            ReferenceId = appointment.Id,
            SentAt = DateTime.UtcNow
        };
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static (string Title, string Body) GetNotificationContent(string status, DateTime scheduledAt)
    {
        var dateStr = $"{scheduledAt:MMM dd, yyyy} at {scheduledAt:HH:mm}";
        return status switch
        {
            "Confirmed" => ("Appointment Confirmed", $"Your appointment on {dateStr} has been confirmed by the doctor."),
            "Rejected" => ("Appointment Declined", $"Your appointment request for {dateStr} was declined. Please check for suggested alternatives."),
            "Completed" => ("Appointment Completed", $"Your appointment on {dateStr} has been marked as completed."),
            "NoShow" => ("Missed Appointment", $"You were marked as a no-show for your appointment on {dateStr}."),
            _ => ("Appointment Updated", $"Your appointment on {dateStr} has been updated.")
        };
    }
}
