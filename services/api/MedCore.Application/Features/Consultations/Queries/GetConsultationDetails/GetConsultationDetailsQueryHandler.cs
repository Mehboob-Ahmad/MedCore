using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Consultations.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Consultations.Queries.GetConsultationDetails;

public class GetConsultationDetailsQueryHandler : IRequestHandler<GetConsultationDetailsQuery, ConsultationDto?>
{
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public GetConsultationDetailsQueryHandler(IGenericRepository<Consultation> consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<ConsultationDto?> Handle(GetConsultationDetailsQuery request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetQueryable().AsNoTracking()
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .Include(c => c.Prescriptions.Where(p => !p.IsDeleted && !p.IsSuperseded))
                .ThenInclude(p => p.Items.Where(i => !i.IsDeleted).OrderBy(i => i.SortOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ConsultationId, cancellationToken);

        if (consultation == null)
            return null;

        // Authorization
        if (consultation.DoctorId != request.UserId && consultation.PatientId != request.UserId)
            return null; // Unauthorized

        var dto = new ConsultationDto
        {
            Id = consultation.Id,
            AppointmentId = consultation.AppointmentId,
            DoctorId = consultation.DoctorId,
            PatientId = consultation.PatientId,
            Date = consultation.CreatedAt, // Date of consultation creation
            DoctorName = $"Dr. {consultation.Doctor.FirstName} {consultation.Doctor.LastName}",
            PatientName = $"{consultation.Patient.FirstName} {consultation.Patient.LastName}",
            ChiefComplaint = consultation.ChiefComplaint,
            Symptoms = consultation.Symptoms,
            Diagnosis = consultation.Diagnosis,
            TreatmentPlan = consultation.TreatmentPlan,
            ClinicalNotes = consultation.ClinicalNotes,
            PatientNotes = consultation.PatientNotes,
            VisitType = consultation.VisitType,
            FollowUpDate = consultation.FollowUpDate,
            FollowUpUrgency = consultation.FollowUpUrgency,
            FollowUpInstructions = consultation.FollowUpInstructions,
            IsFinalized = consultation.IsFinalized,
            FinalizedAt = consultation.FinalizedAt,
            CreatedAt = consultation.CreatedAt,
            UpdatedAt = consultation.UpdatedAt
        };

        // Only doctors can see PrivateNotes
        if (request.IsDoctor && consultation.DoctorId == request.UserId)
        {
            dto.PrivateNotes = consultation.PrivateNotes;
        }
        else
        {
            dto.PrivateNotes = null;
        }

        // Map Prescriptions
        dto.Prescriptions = consultation.Prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            IssuedAt = p.IssuedAt,
            Notes = p.Notes,
            Items = p.Items.Select(i => new PrescriptionItemDto
            {
                Id = i.Id,
                MedicationName = i.MedicationName,
                Strength = i.Strength,
                Dosage = i.Dosage,
                Frequency = i.Frequency,
                Duration = i.Duration,
                Route = i.Route,
                Timing = i.Timing,
                Quantity = i.Quantity,
                Instructions = i.Instructions,
                SortOrder = i.SortOrder
            }).ToList()
        }).ToList();

        return dto;
    }
}
