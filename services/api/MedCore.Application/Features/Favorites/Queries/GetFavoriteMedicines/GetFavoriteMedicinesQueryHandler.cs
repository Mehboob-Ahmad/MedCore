using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Favorites.Queries.GetFavoriteMedicines;

public class GetFavoriteMedicinesQueryHandler : IRequestHandler<GetFavoriteMedicinesQuery, List<string>>
{
    private readonly IGenericRepository<DoctorFavoriteMedicine> _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetFavoriteMedicinesQueryHandler(
        IGenericRepository<DoctorFavoriteMedicine> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<List<string>> Handle(GetFavoriteMedicinesQuery request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        return await _repository.GetQueryable().AsNoTracking()
            .Where(f => f.DoctorId == doctorId)
            .OrderByDescending(f => f.AddedAt)
            .Select(f => f.MedicationName)
            .ToListAsync(cancellationToken);
    }
}
