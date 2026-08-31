using MedicHp.Application.Common;
using MedicHp.Application.Features.Patients.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicHp.Shared.Exceptions;

namespace MedicHp.Application.Features.Patients.Queries.GetPatientProfile;

public class GetPatientProfileQueryHandler : IRequestHandler<GetPatientProfileQuery, PatientProfileDto>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<MedicHp.Domain.Entities.Core.User> _userRepository;

    public GetPatientProfileQueryHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<MedicHp.Domain.Entities.Core.User> userRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _userRepository = userRepository;
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
                .Include(p => p.Medications)
                .Include(p => p.Surgeries)
                .Include(p => p.Hospitalizations),
            cancellationToken);

        if (profile == null)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) throw new UnauthorizedAccessException();
            
            return new PatientProfileDto
            {
                Id = Guid.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                EmergencyContacts = new List<EmergencyContactDto>(),
                Allergies = new List<AllergyDto>(),
                ChronicConditions = new List<ChronicConditionDto>(),
                CurrentMedications = new List<MedicationDto>(),
                Surgeries = new List<SurgeryDto>(),
                Hospitalizations = new List<HospitalizationDto>()
            };
        }

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
            }).ToList(),

            Surgeries = profile.Surgeries.Select(s => new SurgeryDto
            {
                Id = s.Id,
                SurgeryName = s.SurgeryName,
                SurgeryDate = s.SurgeryDate,
                SurgeonName = s.SurgeonName,
                HospitalName = s.HospitalName,
                Notes = s.Notes
            }).ToList(),

            Hospitalizations = profile.Hospitalizations.Select(h => new HospitalizationDto
            {
                Id = h.Id,
                Reason = h.Reason,
                AdmissionDate = h.AdmissionDate,
                DischargeDate = h.DischargeDate,
                HospitalName = h.HospitalName,
                Notes = h.Notes
            }).ToList(),

            FamilyMedicalHistory = profile.FamilyMedicalHistory,
            MedicalHistory = profile.MedicalHistory,
            ImmunizationHistory = profile.ImmunizationHistory,
            LifestyleInformation = profile.LifestyleInformation
        };
    }
}
