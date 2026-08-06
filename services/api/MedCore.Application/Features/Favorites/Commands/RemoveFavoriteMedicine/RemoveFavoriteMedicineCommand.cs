using System;
using MediatR;

namespace MedCore.Application.Features.Favorites.Commands.RemoveFavoriteMedicine;

public class RemoveFavoriteMedicineCommand : IRequest<bool>
{
    public string MedicationName { get; set; } = null!;
}
