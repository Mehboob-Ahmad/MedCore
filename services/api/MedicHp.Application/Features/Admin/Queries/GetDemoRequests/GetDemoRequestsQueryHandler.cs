using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Admin;
using MediatR;

namespace MedicHp.Application.Features.Admin.Queries.GetDemoRequests;

public class GetDemoRequestsQueryHandler : IRequestHandler<GetDemoRequestsQuery, List<DemoRequestDto>>
{
    private readonly IGenericRepository<DemoRequest> _demoRequestRepository;

    public GetDemoRequestsQueryHandler(IGenericRepository<DemoRequest> demoRequestRepository)
    {
        _demoRequestRepository = demoRequestRepository;
    }

    public async Task<List<DemoRequestDto>> Handle(GetDemoRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _demoRequestRepository.GetQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var result = await System.Threading.Tasks.Task.Run(() => query.ToList());

        return result.Select(x => new DemoRequestDto
        {
            Id = x.Id,
            FullName = x.FullName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            Specialization = x.Specialization,
            City = x.City,
            ClinicOrHospital = x.ClinicOrHospital,
            YearsOfExperience = x.YearsOfExperience,
            ProfessionalQualification = x.ProfessionalQualification,
            Status = x.Status,
            CreatedAt = x.CreatedAt
        }).ToList();
    }
}
