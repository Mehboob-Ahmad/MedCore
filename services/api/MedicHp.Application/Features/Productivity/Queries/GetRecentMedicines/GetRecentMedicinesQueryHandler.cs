using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Productivity.Queries.GetRecentMedicines;

public class GetRecentMedicinesQueryHandler : IRequestHandler<GetRecentMedicinesQuery, List<string>>
{
    private readonly IGenericRepository<PrescriptionItem> _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetRecentMedicinesQueryHandler(
        IGenericRepository<PrescriptionItem> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<List<string>> Handle(GetRecentMedicinesQuery request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        // Group by medication name, order by max CreatedAt, then by count
        var recentMedicines = await _repository.GetQueryable().AsNoTracking()
            .Where(pi => pi.Prescription.DoctorId == doctorId && !pi.IsDeleted && !pi.Prescription.IsDeleted)
            .GroupBy(pi => pi.MedicationName.ToLower())
            .Select(g => new 
            {
                MedicationName = g.First().MedicationName,
                LastPrescribed = g.Max(x => x.CreatedAt),
                Frequency = g.Count()
            })
            .OrderByDescending(x => x.LastPrescribed)
            .ThenByDescending(x => x.Frequency)
            .Take(request.Limit)
            .Select(x => x.MedicationName)
            .ToListAsync(cancellationToken);

        return recentMedicines;
    }
}
