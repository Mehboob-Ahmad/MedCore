using MedCore.Application.Common;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Shared.Exceptions;
using System;

namespace MedCore.Application.Features.Patients.Commands.UpdatePatientProfile;

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
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);

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

        // Calculate Profile Completion Percentage
        profile.ProfileCompletionPct = CalculateCompletionPercentage(profile);

        await _patientProfileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private int CalculateCompletionPercentage(PatientProfile profile)
    {
        int totalFields = 6;
        int filledFields = 0;

        if (profile.DateOfBirth.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.Gender)) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.BloodType)) filledFields++;
        if (profile.CityId.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(profile.Address)) filledFields++;
        if (profile.DataSharingConsent) filledFields++; // Just an example logic

        return (int)Math.Round((double)filledFields / totalFields * 100);
    }
}
