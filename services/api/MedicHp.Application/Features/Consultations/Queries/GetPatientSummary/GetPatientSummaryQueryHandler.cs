using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Consultations.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Consultations.Queries.GetPatientSummary;

public class GetPatientSummaryQueryHandler : IRequestHandler<GetPatientSummaryQuery, PatientSummaryDto?>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public GetPatientSummaryQueryHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<Consultation> consultationRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _consultationRepository = consultationRepository;
    }

    public async Task<PatientSummaryDto?> Handle(GetPatientSummaryQuery request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.GetQueryable().AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.ProfilePhotoFile)
            .Include(p => p.EmergencyContacts)
            .Include(p => p.Allergies.Where(a => !a.IsDeleted))
            .Include(p => p.ChronicConditions.Where(c => !c.IsDeleted))
            .Include(p => p.Medications.Where(m => !m.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.PatientId, cancellationToken);

        if (profile == null)
            return null;

        var lastConsultation = await _consultationRepository.GetQueryable().AsNoTracking()
            .Where(c => c.PatientId == request.PatientId && c.IsFinalized)
            .OrderByDescending(c => c.FinalizedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        var upcomingFollowUp = await _consultationRepository.GetQueryable().AsNoTracking()
            .Where(c => c.PatientId == request.PatientId && c.FollowUpDate > DateTime.UtcNow)
            .OrderBy(c => c.FollowUpDate)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        var ec = profile.EmergencyContacts.FirstOrDefault();

        return new PatientSummaryDto
        {
            PatientId = profile.UserId,
            FullName = $"{profile.User.FirstName} {profile.User.LastName}",
            Age = profile.DateOfBirth.HasValue ? DateTime.UtcNow.Year - profile.DateOfBirth.Value.Year : 0,
            Gender = profile.Gender ?? "Unknown",
            BloodGroup = profile.BloodType ?? "Unknown",
            ProfilePhotoUrl = profile.User.ProfilePhotoFile?.StoragePath,
            EmergencyContact = ec != null ? new EmergencyContactDto
            {
                Name = ec.FullName,
                Relationship = ec.Relationship,
                PhoneNumber = ec.PhoneNumber
            } : null,
            KnownAllergies = profile.Allergies.Select(a => a.AllergyName).ToList(),
            ChronicConditions = profile.ChronicConditions.Select(c => c.ConditionName).ToList(),
            CurrentMedications = profile.Medications.Select(m => m.MedicationName).ToList(),
            LastConsultationDate = lastConsultation?.FinalizedAt,
            LastConsultationDiagnosis = lastConsultation?.Diagnosis,
            UpcomingFollowUpDate = upcomingFollowUp?.FollowUpDate
        };
    }
}
