using MedCore.Application.Common;
using MedCore.Application.Features.Patients.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedCore.Shared.Exceptions;

namespace MedCore.Application.Features.Patients.Queries.GetPatientProfile;

public class GetPatientProfileQueryHandler : IRequestHandler<GetPatientProfileQuery, PatientProfileDto>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;

    public GetPatientProfileQueryHandler(IGenericRepository<PatientProfile> patientProfileRepository)
    {
        _patientProfileRepository = patientProfileRepository;
    }

    public async Task<PatientProfileDto> Handle(GetPatientProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            include: q => q
                .Include(p => p.User)
                .Include(p => p.City)
                .Include(p => p.EmergencyContacts)
                .Include(p => p.Allergies)
                .Include(p => p.ChronicConditions)
                .Include(p => p.Medications),
            cancellationToken);

        if (profile == null)
            throw new NotFoundException(nameof(PatientProfile), request.UserId);

        return new PatientProfileDto
        {
            Id = profile.Id,
            FirstName = profile.User.FirstName,
            LastName = profile.User.LastName,
            Email = profile.User.Email,
            PhoneNumber = profile.User.PhoneNumber,
            ProfilePhotoUrl = null, // TODO: ProfilePhoto
            DateOfBirth = profile.DateOfBirth?.ToDateTime(new System.TimeOnly(0, 0)),
            Gender = profile.Gender,
            BloodType = profile.BloodType,
            City = profile.City != null ? new CityDto { Id = profile.City.Id, Name = profile.City.Name } : null,
            Address = profile.Address,
            DataSharingConsent = profile.DataSharingConsent,
            ProfileCompletionPct = profile.ProfileCompletionPct,
            CreatedAt = profile.CreatedAt,
            
            EmergencyContacts = profile.EmergencyContacts.Select(ec => new EmergencyContactDto
            {
                Id = ec.Id,
                FullName = ec.FullName,
                Relationship = ec.Relationship,
                PhoneNumber = ec.PhoneNumber,
                IsPrimary = ec.IsPrimary
            }).ToList(),
            
            Allergies = profile.Allergies.Select(a => new AllergyDto
            {
                Id = a.Id,
                AllergyName = a.AllergyName,
                Severity = a.Severity,
                Notes = a.Notes
            }).ToList(),
            
            ChronicConditions = profile.ChronicConditions.Select(cc => new ChronicConditionDto
            {
                Id = cc.Id,
                ConditionName = cc.ConditionName,
                DiagnosedDate = cc.DiagnosedDate?.ToDateTime(new System.TimeOnly(0, 0)),
                Notes = cc.Notes
            }).ToList(),
            
            CurrentMedications = profile.Medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                MedicationName = m.MedicationName,
                Dosage = m.Dosage,
                Frequency = m.Frequency
            }).ToList()
        };
    }
}
