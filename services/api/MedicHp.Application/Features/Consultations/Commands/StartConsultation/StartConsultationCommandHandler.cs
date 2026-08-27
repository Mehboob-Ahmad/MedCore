using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Consultations.Commands.StartConsultation;

public class StartConsultationCommandHandler : IRequestHandler<StartConsultationCommand, Guid>
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public StartConsultationCommandHandler(
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<Consultation> consultationRepository)
    {
        _appointmentRepository = appointmentRepository;
        _consultationRepository = consultationRepository;
    }

    public async Task<Guid> Handle(StartConsultationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetQueryable()
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.DoctorId == request.DoctorId, cancellationToken);

        if (appointment == null)
            throw new Exception("Appointment not found or unauthorized.");

        // Check if consultation already exists
        var existingConsultation = await _consultationRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.AppointmentId == request.AppointmentId, cancellationToken);

        if (existingConsultation != null)
            return existingConsultation.Id; // Idempotent

        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            DoctorId = appointment.DoctorId,
            PatientId = appointment.PatientId,
            ChiefComplaint = string.Empty, // Draft state
            Symptoms = string.Empty,
            Diagnosis = string.Empty,
            TreatmentPlan = string.Empty,
            IsFinalized = false
        };

        await _consultationRepository.AddAsync(consultation, cancellationToken);
        
        // Optionally update appointment status to InProgress
        if (appointment.Status == "Pending" || appointment.Status == "Confirmed")
        {
            appointment.Status = "InProgress";
            appointment.StatusHistory.Add(new AppointmentStatusHistory
            {
                FromStatus = appointment.Status,
                ToStatus = "InProgress",
                ChangedByUserId = request.DoctorId,
                Reason = "Consultation started"
            });
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        }

        return consultation.Id;
    }
}
