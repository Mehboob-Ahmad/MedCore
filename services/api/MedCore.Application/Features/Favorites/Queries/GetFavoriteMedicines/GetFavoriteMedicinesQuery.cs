using System.Collections.Generic;
using MediatR;

namespace MedCore.Application.Features.Favorites.Queries.GetFavoriteMedicines;

public class GetFavoriteMedicinesQuery : IRequest<List<string>>
{
}
