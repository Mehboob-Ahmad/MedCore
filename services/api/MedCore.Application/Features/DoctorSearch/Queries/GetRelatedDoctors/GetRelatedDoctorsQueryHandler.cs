using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.DoctorSearch.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.DoctorSearch.Queries.GetRelatedDoctors;

public class GetRelatedDoctorsQueryHandler : IRequestHandler<GetRelatedDoctorsQuery, List<DoctorSearchResultDto>>
{
    private readonly IGenericRepository<DoctorProfile> _doctorRepository;

    public GetRelatedDoctorsQueryHandler(IGenericRepository<DoctorProfile> doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<List<DoctorSearchResultDto>> Handle(GetRelatedDoctorsQuery request, CancellationToken cancellationToken)
    {
        // Get target doctor's specializations and city
        var targetDoctor = await _doctorRepository.GetQueryable().AsNoTracking()
            .Include(d => d.Specializations)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == request.DoctorId, cancellationToken);

        if (targetDoctor == null) return new List<DoctorSearchResultDto>();

        var specializationIds = targetDoctor.Specializations.Select(s => s.SpecializationId).ToList();

        var query = _doctorRepository.GetQueryable().AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.City)
            .Include(d => d.Specializations)
                .ThenInclude(s => s.Specialization)
            .AsNoTracking()
            .Where(d => d.UserId != request.DoctorId && d.VerificationStatus == "Verified" && d.User.IsActive);

        // Find doctors with matching specializations
        if (specializationIds.Any())
        {
            query = query.Where(d => d.Specializations.Any(s => specializationIds.Contains(s.SpecializationId)));
        }

        // We can sort by city match to prioritize local doctors
        var relatedDoctors = await query
            .OrderByDescending(d => d.CityId == targetDoctor.CityId) // True first
            .ThenByDescending(d => d.YearsOfExperience)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return relatedDoctors.Select(d => new DoctorSearchResultDto
        {
            DoctorId = d.UserId,
            FullName = $"Dr. {d.User.FirstName} {d.User.LastName}",
            ProfilePhotoUrl = d.User?.ProfilePhotoFile?.StoragePath,
            Bio = d.Bio,
            ConsultationFee = d.ConsultationFee,
            YearsOfExperience = d.YearsOfExperience,
            CityName = d.City?.Name,
            ClinicName = d.ClinicName,
            Languages = d.Languages,
            Gender = d.User.Gender,
            Specializations = d.Specializations.Select(s => s.Specialization.Name).ToList()
        }).ToList();
    }
}
