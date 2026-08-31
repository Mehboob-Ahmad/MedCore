using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using System;

namespace MedicHp.Application.Features.Patients.Commands.UpdatePatientProfile;

public class UpdatePatientProfileCommandHandler : IRequestHandler<UpdatePatientProfileCommand, bool>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePatientProfileCommandHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePatientProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId, 
            q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
                Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(q, p => p.Surgeries), 
                p => p.Hospitalizations), 
            cancellationToken);

        if (profile == null)
            throw new NotFoundException(nameof(PatientProfile), request.UserId);

        if (request.DateOfBirth.HasValue)
            profile.DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth.Value);
        
        if (request.Gender != null)
            profile.Gender = request.Gender;
            
        if (request.BloodType != null)
            profile.BloodType = request.BloodType;
            
        if (request.CityId.HasValue)
            profile.CityId = request.CityId.Value;
            
        if (request.Address != null)
            profile.Address = request.Address;
            
        if (request.DataSharingConsent.HasValue)
            profile.DataSharingConsent = request.DataSharingConsent.Value;

        if (request.FamilyMedicalHistory != null)
            profile.FamilyMedicalHistory = request.FamilyMedicalHistory;

        if (request.MedicalHistory != null)
            profile.MedicalHistory = request.MedicalHistory;

        if (request.ImmunizationHistory != null)
            profile.ImmunizationHistory = request.ImmunizationHistory;

        if (request.LifestyleInformation != null)
            profile.LifestyleInformation = request.LifestyleInformation;

        if (request.Surgeries != null)
        {
            profile.Surgeries.Clear();
            foreach(var s in request.Surgeries)
            {
                profile.Surgeries.Add(new PatientSurgery {
                    SurgeryName = s.SurgeryName,
                    SurgeryDate = s.SurgeryDate,
                    SurgeonName = s.SurgeonName,
                    HospitalName = s.HospitalName,
                    Notes = s.Notes
                });
            }
        }

        if (request.Hospitalizations != null)
        {
            profile.Hospitalizations.Clear();
            foreach(var h in request.Hospitalizations)
            {
                profile.Hospitalizations.Add(new PatientHospitalization {
                    Reason = h.Reason,
                    AdmissionDate = h.AdmissionDate,
                    DischargeDate = h.DischargeDate,
                    HospitalName = h.HospitalName,
                    Notes = h.Notes
                });
            }
        }

        // Calculate Profile Completion Percentage
        profile.ProfileCompletionPct = CalculateCompletionPercentage(profile);

        await _patientProfileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private int CalculateCompletionPercentage(PatientProfile profile)
    {
        int totalFields = 10;
        int filledFields = 0;

        if (profile.DateOfBirth.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.Gender)) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.BloodType)) filledFields++;
        if (profile.CityId.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.Address)) filledFields++;
        if (profile.DataSharingConsent) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.FamilyMedicalHistory)) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.MedicalHistory)) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.ImmunizationHistory)) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.LifestyleInformation)) filledFields++;

        return (int)Math.Round((double)filledFields / totalFields * 100);
    }
}
