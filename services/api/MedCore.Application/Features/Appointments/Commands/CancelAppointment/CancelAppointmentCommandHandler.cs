using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MedCore.Domain.Entities.Core;
using MediatR;
using MedCore.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, bool>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<AppointmentStatusHistory> _statusHistoryRepository;
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAppointmentCommandHandler(
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

    public async Task<bool> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new NotFoundException(nameof(Appointment), request.Id);

        // Authorization: only the patient or the doctor can cancel
        if (appointment.PatientId != request.UserId && appointment.DoctorId != request.UserId)
            throw new NotFoundException(nameof(Appointment), request.Id);

        // Business rule: cannot cancel already completed or cancelled appointments
        if (appointment.Status is "Cancelled" or "Completed" or "NoShow")
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Status",
                    $"Appointment with status '{appointment.Status}' cannot be cancelled.")
            });

        var previousStatus = appointment.Status;
        appointment.Status = "Cancelled";
        appointment.CancellationReason = request.Reason;

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        // Record status history
        var statusHistory = new AppointmentStatusHistory
        {
            AppointmentId = appointment.Id,
            FromStatus = previousStatus,
            ToStatus = "Cancelled",
            ChangedByUserId = request.UserId,
            Reason = request.Reason
        };
        await _statusHistoryRepository.AddAsync(statusHistory, cancellationToken);

        // Notify the other party
        var notifyUserId = appointment.PatientId == request.UserId
            ? appointment.DoctorId
            : appointment.PatientId;

        var notification = new Notification
        {
            UserId = notifyUserId,
            Type = "AppointmentCancelled",
            Title = "Appointment Cancelled",
            Body = $"An appointment on {appointment.ScheduledAt:MMM dd, yyyy} at {appointment.ScheduledAt:HH:mm} has been cancelled.",
            ReferenceType = "Appointment",
            ReferenceId = appointment.Id,
            SentAt = DateTime.UtcNow
        };
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
