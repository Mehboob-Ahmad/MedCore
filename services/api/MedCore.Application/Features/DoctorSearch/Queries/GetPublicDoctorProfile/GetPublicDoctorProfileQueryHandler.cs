using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.DoctorSearch.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.DoctorSearch.Queries.GetPublicDoctorProfile;

public class GetPublicDoctorProfileQueryHandler : IRequestHandler<GetPublicDoctorProfileQuery, DoctorPublicProfileDto?>
{
    private readonly IGenericRepository<DoctorProfile> _doctorRepository;

    public GetPublicDoctorProfileQueryHandler(IGenericRepository<DoctorProfile> doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<DoctorPublicProfileDto?> Handle(GetPublicDoctorProfileQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetQueryable().AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.City)
            .Include(d => d.Specializations)
                .ThenInclude(s => s.Specialization)
            .Include(d => d.Availabilities)
            .Where(d => d.VerificationStatus == "Verified" && d.User.IsActive)
            .FirstOrDefaultAsync(d => d.UserId == request.DoctorId, cancellationToken);

        if (doctor == null) return null;

        return new DoctorPublicProfileDto
        {
            DoctorId = doctor.UserId,
            FullName = $"Dr. {doctor.User.FirstName} {doctor.User.LastName}",
            ProfilePhotoUrl = doctor.User.ProfilePhotoFile?.StoragePath,
            Bio = doctor.Bio,
            LicenseNumber = doctor.LicenseNumber, // Can be masked depending on requirements, but often public
            ConsultationFee = doctor.ConsultationFee,
            YearsOfExperience = doctor.YearsOfExperience,
            CityName = doctor.City?.Name,
            Address = doctor.Address,
            ClinicName = doctor.ClinicName,
            Languages = doctor.Languages,
            Gender = doctor.User.Gender,
            Specializations = doctor.Specializations.Select(s => s.Specialization.Name).ToList(),
            IsAcceptingNewPatients = true, // Logic could be added to calculate from schedule
            Availabilities = doctor.Availabilities.Select(a => new DoctorAvailabilityDto
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList()
        };
    }
}
