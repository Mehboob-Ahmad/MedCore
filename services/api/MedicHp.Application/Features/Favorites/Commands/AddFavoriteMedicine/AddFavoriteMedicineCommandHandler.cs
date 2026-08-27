using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Favorites.Commands.AddFavoriteMedicine;

public class AddFavoriteMedicineCommandHandler : IRequestHandler<AddFavoriteMedicineCommand, Guid>
{
    private readonly IGenericRepository<DoctorFavoriteMedicine> _repository;
    private readonly ICurrentUserService _currentUserService;

    public AddFavoriteMedicineCommandHandler(
        IGenericRepository<DoctorFavoriteMedicine> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(AddFavoriteMedicineCommand request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        var existing = await _repository.GetQueryable()
            .FirstOrDefaultAsync(f => f.DoctorId == doctorId && f.MedicationName.ToLower() == request.MedicationName.ToLower(), cancellationToken);
            
        if (existing != null)
        {
            return existing.Id; // Idempotent
        }

        var favorite = new DoctorFavoriteMedicine
        {
            DoctorId = doctorId,
            MedicationName = request.MedicationName,
            AddedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(favorite, cancellationToken);

        return favorite.Id;
    }
}
