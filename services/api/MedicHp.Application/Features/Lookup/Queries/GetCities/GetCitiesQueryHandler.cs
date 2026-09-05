using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Lookup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Lookup.Queries.GetCities;

public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, List<CityDto>>
{
    private readonly IGenericRepository<City> _cityRepository;

    public GetCitiesQueryHandler(IGenericRepository<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<List<CityDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var query = _cityRepository.GetQueryable();
        
        var cities = await query
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                StateOrProvince = c.StateOrProvince,
                Country = c.Country
            })
            .ToListAsync(cancellationToken);

        return cities;
    }
}
