using System;
using System.Collections.Generic;
using MediatR;

namespace MedicHp.Application.Features.Lookup.Queries.GetCities;

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StateOrProvince { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class GetCitiesQuery : IRequest<List<CityDto>>
{
}
