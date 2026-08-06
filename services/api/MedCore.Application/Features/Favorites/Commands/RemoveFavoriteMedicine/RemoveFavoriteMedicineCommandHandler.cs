using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Favorites.Commands.RemoveFavoriteMedicine;

public class RemoveFavoriteMedicineCommandHandler : IRequestHandler<RemoveFavoriteMedicineCommand, bool>
{
    private readonly IGenericRepository<DoctorFavoriteMedicine> _repository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFavoriteMedicineCommandHandler(
        IGenericRepository<DoctorFavoriteMedicine> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(RemoveFavoriteMedicineCommand request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;

        var existing = await _repository.GetQueryable()
            .FirstOrDefaultAsync(f => f.DoctorId == doctorId && f.MedicationName.ToLower() == request.MedicationName.ToLower(), cancellationToken);
            
        if (existing == null)
        {
            return false;
        }

        await _repository.DeleteAsync(existing, cancellationToken);
        return true;
    }
}
