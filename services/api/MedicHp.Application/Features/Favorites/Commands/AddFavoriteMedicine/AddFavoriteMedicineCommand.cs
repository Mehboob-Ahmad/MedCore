using System;
using MediatR;

namespace MedicHp.Application.Features.Favorites.Commands.AddFavoriteMedicine;

public class AddFavoriteMedicineCommand : IRequest<Guid>
{
    public string MedicationName { get; set; } = null!;
}
