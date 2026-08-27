using System.Collections.Generic;
using MediatR;

namespace MedicHp.Application.Features.Favorites.Queries.GetFavoriteMedicines;

public class GetFavoriteMedicinesQuery : IRequest<List<string>>
{
}
