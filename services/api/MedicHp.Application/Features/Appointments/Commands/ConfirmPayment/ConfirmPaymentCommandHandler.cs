using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Enums;
using MediatR;

namespace MedicHp.Application.Features.Appointments.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, bool>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<MedicHp.Domain.Entities.Core.Notification> _notificationRepository;
    private readonly IWhatsAppNotificationService _whatsAppNotificationService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPaymentCommandHandler(
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<MedicHp.Domain.Entities.Core.Notification> notificationRepository,
        IWhatsAppNotificationService whatsAppNotificationService,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _notificationRepository = notificationRepository;
        _whatsAppNotificationService = whatsAppNotificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment == null)
            throw new NotFoundException(nameof(Appointment), request.AppointmentId);

        // Ensure the doctor owns this appointment
        if (appointment.DoctorId != request.DoctorId)
            throw new UnauthorizedAccessException("You are not authorized to confirm payment for this appointment.");

        if (appointment.PaymentStatus == PaymentStatus.Paid.ToString())
            throw new InvalidOperationException("Payment is already confirmed for this appointment.");

        appointment.PaymentStatus = PaymentStatus.Paid.ToString();
        appointment.PaymentConfirmedAt = DateTime.UtcNow;
        appointment.PaymentConfirmedByUserId = request.DoctorId;

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        // Create in-app notification for patient
        var notification = new MedicHp.Domain.Entities.Core.Notification
        {
            UserId = appointment.PatientId,
            Title = "Payment Confirmed",
            Body = $"Your payment for appointment on {appointment.ScheduledAt:MMM dd, yyyy} has been confirmed.",
            Type = "Payment"
        };
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send WhatsApp notification asynchronously (fire and forget pattern is already handled inside the service if not awaited, but we await it here)
        // Since WhatsAppNotificationService fetches details on its own, we just pass the ID
        try
        {
            await _whatsAppNotificationService.SendPaymentSuccessAsync(appointment.Id);
        }
        catch (Exception)
        {
            // Log and ignore to prevent transaction rollback if WhatsApp fails
            // In a robust system, we would enqueue this.
        }

        return true;
    }
}
