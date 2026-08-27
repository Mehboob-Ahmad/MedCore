using MedicHp.Application.Common;
using MedicHp.Application.Features.Doctors.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Doctors.Queries.SearchDoctors;

public class SearchDoctorsQueryHandler : IRequestHandler<SearchDoctorsQuery, List<DoctorSearchDto>>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;

    public SearchDoctorsQueryHandler(IGenericRepository<DoctorProfile> doctorProfileRepository)
    {
        _doctorProfileRepository = doctorProfileRepository;
    }

    public async Task<List<DoctorSearchDto>> Handle(SearchDoctorsQuery request, CancellationToken cancellationToken)
    {
        // Fetch doctors and filter in-memory for simplicity in Phase 1, or use Expressions.
        var doctors = await _doctorProfileRepository.GetAsync(
            d => true, // Assuming soft delete handles IsDeleted
            include: q => q.Include(d => d.User).Include(d => d.Specializations),
            cancellationToken);

        var query = doctors.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(d => 
                (d.User?.FirstName?.ToLower().Contains(term) ?? false) ||
                (d.User?.LastName?.ToLower().Contains(term) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(request.Specialty))
        {
            var spec = request.Specialty.ToLower();
            // TODO: Proper specialization mapping
        }

        // Mock rating
        var rnd = new System.Random();

        return query.Select(d => new DoctorSearchDto
        {
            DoctorId = d.Id,
            FirstName = d.User?.FirstName ?? "",
            LastName = d.User?.LastName ?? "",
            ProfilePhotoUrl = null,
            ExperienceYears = d.YearsOfExperience,
            Bio = d.Bio ?? "",
            ConsultationFee = d.ConsultationFee,
            ProfessionalType = d.ProfessionalType,
            Specializations = new List<string> { "General Medicine" }, // Mocked for now
            Rating = 4.0 + (rnd.NextDouble()),
            ReviewCount = rnd.Next(10, 500)
        }).ToList();
    }
}
