using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Shared.Exceptions;
using MedCore.Application.Common;
using MedCore.Application.Features.Doctors.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Doctors.Queries.GetDoctorProfile;

public class GetDoctorProfileQueryHandler : IRequestHandler<GetDoctorProfileQuery, DoctorProfileDto>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;

    public GetDoctorProfileQueryHandler(IGenericRepository<DoctorProfile> doctorProfileRepository)
    {
        _doctorProfileRepository = doctorProfileRepository;
    }

    public async Task<DoctorProfileDto> Handle(GetDoctorProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            include: q => q.Include(p => p.User)
                           .Include(p => p.Specializations).ThenInclude(s => s.Specialization)
                           .Include(p => p.Availabilities),
            cancellationToken: cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(DoctorProfile), request.UserId);
        }

        return new DoctorProfileDto
        {
            Id = profile.Id,
            FirstName = profile.User?.FirstName ?? string.Empty,
            LastName = profile.User?.LastName ?? string.Empty,
            Email = profile.User?.Email ?? string.Empty,
            PhoneNumber = profile.User?.PhoneNumber ?? string.Empty,
            Bio = profile.Bio ?? string.Empty,
            ConsultationFee = profile.ConsultationFee,
            MedicalLicenseNumber = profile.LicenseNumber,
            ExperienceYears = profile.YearsOfExperience,
            Specializations = profile.Specializations.Select(s => s.Specialization?.Name ?? string.Empty).ToList(),
            Qualifications = new System.Collections.Generic.List<string>(), // We don't have Qualifications in DoctorProfile
            
            Availabilities = profile.Availabilities.Select(a => new DoctorAvailabilityDto
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime.ToString(@"hh\:mm"),
                EndTime = a.EndTime.ToString(@"hh\:mm")
            }).ToList()
        };
    }
}
