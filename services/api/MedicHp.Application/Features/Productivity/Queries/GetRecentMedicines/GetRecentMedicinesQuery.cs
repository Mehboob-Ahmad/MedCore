using System.Collections.Generic;
using MediatR;

namespace MedicHp.Application.Features.Productivity.Queries.GetRecentMedicines;

public class GetRecentMedicinesQuery : IRequest<List<string>>
{
    public int Limit { get; set; } = 10;
}
