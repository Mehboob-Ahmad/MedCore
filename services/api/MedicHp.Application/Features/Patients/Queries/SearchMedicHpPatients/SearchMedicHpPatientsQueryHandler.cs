using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Patients.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Patients.Queries.SearchMedicHpPatients;

public class SearchMedicHpPatientsQueryHandler : IRequestHandler<SearchMedicHpPatientsQuery, List<PatientSearchDto>>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;

    public SearchMedicHpPatientsQueryHandler(IGenericRepository<PatientProfile> patientProfileRepository)
    {
        _patientProfileRepository = patientProfileRepository;
    }

    public async Task<List<PatientSearchDto>> Handle(SearchMedicHpPatientsQuery request, CancellationToken cancellationToken)
    {
        var query = await _patientProfileRepository.GetAsync(
            p => true, // We might want to filter this better later, but for now we search all
            include: q => q.Include(p => p.User),
            cancellationToken: cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(p => 
                (p.User != null && p.User.FirstName.ToLower().Contains(term)) ||
                (p.User != null && p.User.LastName.ToLower().Contains(term)) ||
                (p.User != null && p.User.Email.ToLower().Contains(term)) ||
                (p.User != null && p.User.PhoneNumber.Contains(term))).ToList();
        }

        return query.Select(p => new PatientSearchDto
        {
            PatientId = p.UserId,
            FirstName = p.User?.FirstName ?? "",
            LastName = p.User?.LastName ?? "",
            Email = p.User?.Email ?? "",
            PhoneNumber = p.User?.PhoneNumber ?? "",
            DateOfBirth = p.DateOfBirth.HasValue 
                ? p.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue) 
                : DateTime.MinValue
        }).Take(20).ToList();
    }
}
