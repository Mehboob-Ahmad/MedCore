using System;
using MediatR;

namespace MedCore.Application.Features.Favorites.Commands.AddFavoriteMedicine;

public class AddFavoriteMedicineCommand : IRequest<Guid>
{
    public string MedicationName { get; set; } = null!;
}
