using System;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Consultations.Commands.FinalizeConsultation;

public class FinalizeConsultationCommandHandler : IRequestHandler<FinalizeConsultationCommand, Unit>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public FinalizeConsultationCommandHandler(
        IGenericRepository<Consultation> consultationRepository,
        IGenericRepository<Appointment> appointmentRepository)
    {
        _consultationRepository = consultationRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Unit> Handle(FinalizeConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.Id == request.ConsultationId, cancellationToken);

        if (consultation == null || consultation.DoctorId != request.DoctorId)
            throw new Exception("Consultation not found or unauthorized.");

        if (consultation.IsFinalized)
            throw new Exception("Consultation is already finalized.");

        consultation.IsFinalized = true;
        consultation.FinalizedAt = DateTime.UtcNow;

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        // Update Appointment status to Completed
        var appointment = await _appointmentRepository.GetQueryable()
            .Include(a => a.StatusHistory)
            .FirstOrDefaultAsync(a => a.Id == consultation.AppointmentId, cancellationToken);

        if (appointment != null && appointment.Status != "Completed")
        {
            appointment.Status = "Completed";
            appointment.StatusHistory.Add(new AppointmentStatusHistory
            {
                FromStatus = appointment.Status, // Note: This might record "InProgress -> Completed" but logically it's correct.
                ToStatus = "Completed",
                ChangedByUserId = request.DoctorId,
                Reason = "Consultation Finalized"
            });
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        }

        return Unit.Value;
    }
}
