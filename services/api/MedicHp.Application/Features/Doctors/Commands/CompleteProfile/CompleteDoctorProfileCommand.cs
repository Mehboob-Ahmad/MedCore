using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedicHp.Domain.Entities.Lookup;

namespace MedicHp.Application.Features.Doctors.Commands.CompleteProfile;

public class CompleteDoctorProfileCommand : IRequest<bool>
{
    public List<Guid> SpecializationIds { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseAuthority { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public int YearsOfExperience { get; set; }
    public string AvailabilityHours { get; set; } = string.Empty;
}

public class CompleteDoctorProfileCommandHandler : IRequestHandler<CompleteDoctorProfileCommand, bool>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<MedicHp.Domain.Entities.Core.User> _userRepository;
    private readonly IGenericRepository<Specialization> _specializationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CompleteDoctorProfileCommandHandler(
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<MedicHp.Domain.Entities.Core.User> userRepository,
        IGenericRepository<Specialization> specializationRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _doctorProfileRepository = doctorProfileRepository;
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CompleteDoctorProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user != null && !string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            user.PhoneNumber = request.PhoneNumber;
            await _userRepository.UpdateAsync(user);
        }

        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            dp => dp.UserId == userId.Value,
            include: q => q.Include(dp => dp.Specializations));

        if (profile == null)
            throw new Exception("Doctor profile not found.");

        profile.ClinicName = request.ClinicName;
        profile.Address = request.Address;
        profile.ConsultationFee = request.ConsultationFee;
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.RegistrationNumber = request.RegistrationNumber; // if they provide it
        // We might want to store LicenseAuthority as well, assuming it's added to DoctorProfile entity or just ignoring it for now if it's not in the entity.
        // There is no LicenseAuthority in DoctorProfile currently, we will just set what we can.

        // Update specializations
        profile.Specializations.Clear();
        foreach (var specId in request.SpecializationIds)
        {
            var exists = await _specializationRepository.GetByIdAsync(specId) != null;
            if (exists)
            {
                profile.Specializations.Add(new DoctorSpecialization
                {
                    SpecializationId = specId,
                    DoctorProfileId = profile.Id
                });
            }
        }

        // Simplistic approach for availability hours as string - we don't have a direct field for string availability
        // Usually, we'd add it to DoctorAvailability entity, but since it's just a string, we might save it to Bio for now
        // if no specific field exists, or create a new field. We will use Bio.
        if (!string.IsNullOrWhiteSpace(request.AvailabilityHours))
        {
            profile.Bio = $"Availability: {request.AvailabilityHours}";
        }

        await _doctorProfileRepository.UpdateAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
