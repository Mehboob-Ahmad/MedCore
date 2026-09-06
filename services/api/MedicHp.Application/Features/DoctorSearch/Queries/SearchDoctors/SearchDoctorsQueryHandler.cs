using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.DoctorSearch.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.DoctorSearch.Queries.SearchDoctors;

public class SearchDoctorsQueryHandler : IRequestHandler<SearchDoctorsQuery, DoctorSearchResponseDto>
{
    private readonly IGenericRepository<DoctorProfile> _doctorRepository;

    public SearchDoctorsQueryHandler(IGenericRepository<DoctorProfile> doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<DoctorSearchResponseDto> Handle(SearchDoctorsQuery request, CancellationToken cancellationToken)
    {
        // Use IQueryable to build the query dynamically
        var query = _doctorRepository.GetQueryable().AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.City)
            .Include(d => d.Specializations)
                .ThenInclude(s => s.Specialization)
                    .ThenInclude(s => s.DiseaseSpecializations)
                        .ThenInclude(ds => ds.Disease)
            .Include(d => d.Specializations)
                .ThenInclude(s => s.Specialization)
                    .ThenInclude(s => s.SymptomSpecializations)
                        .ThenInclude(ss => ss.Symptom)
            .Where(d => d.VerificationStatus == "Verified" && d.User.AccountStatus == MedicHp.Domain.Enums.AccountStatus.Active);

        // 1. Text Search (Doctor Name or Bio)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(d => 
                (d.User.FirstName + " " + d.User.LastName).ToLower().Contains(searchTerm) ||
                (d.Bio != null && d.Bio.ToLower().Contains(searchTerm)));
        }

        // 2. City
        if (request.CityIds != null && request.CityIds.Any())
        {
            query = query.Where(d => d.CityId.HasValue && request.CityIds.Contains(d.CityId.Value));
        }

        // 3. Specialization Filter
        if (!string.IsNullOrWhiteSpace(request.Specialization))
        {
            var spec = request.Specialization.ToLower();
            query = query.Where(d => d.Specializations.Any(s => s.Specialization.Name.ToLower().Contains(spec)));
        }

        // 4. Disease / Symptom Mapping
        if (!string.IsNullOrWhiteSpace(request.Disease))
        {
            var disease = request.Disease.ToLower();
            query = query.Where(d => d.Specializations.Any(s => 
                s.Specialization.DiseaseSpecializations.Any(ds => ds.Disease.Name.ToLower().Contains(disease))));
        }

        if (!string.IsNullOrWhiteSpace(request.Symptom))
        {
            var symptom = request.Symptom.ToLower();
            query = query.Where(d => d.Specializations.Any(s => 
                s.Specialization.SymptomSpecializations.Any(ss => ss.Symptom.Name.ToLower().Contains(symptom))));
        }

        // 5. Additional Filters
        if (!string.IsNullOrWhiteSpace(request.ClinicName))
        {
            query = query.Where(d => d.ClinicName != null && d.ClinicName.ToLower().Contains(request.ClinicName.ToLower()));
        }
        
        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            query = query.Where(d => d.Languages != null && d.Languages.ToLower().Contains(request.Language.ToLower()));
        }

        if (request.MaxFee.HasValue)
        {
            query = query.Where(d => d.ConsultationFee <= request.MaxFee.Value);
        }

        if (request.MinExperienceYears.HasValue)
        {
            query = query.Where(d => d.YearsOfExperience >= request.MinExperienceYears.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Gender))
        {
            query = query.Where(d => d.User.Gender == request.Gender);
        }

        // Fetch Data and Apply Relevance / Sorting
        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "lowestfee" => query.OrderBy(d => d.ConsultationFee),
            "highestexperience" => query.OrderByDescending(d => d.YearsOfExperience),
            "newest" => query.OrderByDescending(d => d.CreatedAt),
            "alphabetical" => query.OrderBy(d => d.User.FirstName).ThenBy(d => d.User.LastName),
            _ => query.OrderByDescending(d => d.YearsOfExperience) // Default / Relevance
        };

        var pagedDoctors = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new DoctorSearchResponseDto
        {
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Doctors = pagedDoctors.Select(d => new DoctorSearchResultDto
            {
                DoctorId = d.UserId,
                FullName = $"Dr. {d.User.FirstName} {d.User.LastName}",
                ProfilePhotoUrl = d.User?.ProfilePhotoFile?.StoragePath, // Assuming File has Url
                Bio = d.Bio,
                ConsultationFee = d.ConsultationFee,
                YearsOfExperience = d.YearsOfExperience,
                CityName = d.City?.Name,
                ClinicName = d.ClinicName,
                Languages = d.Languages,
                Gender = d.User.Gender,
                Specializations = d.Specializations.Select(s => s.Specialization.Name).ToList(),
                RelevanceScore = 0 // In a real scenario, calculate based on match exactness
            }).ToList()
        };

        return result;
    }
}
