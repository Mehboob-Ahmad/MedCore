using System.Threading.Tasks;
using MedicHp.Application.Features.Lookup.Queries.GetCities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/system")]
[AllowAnonymous] // Cities should be accessible without auth for the Find Doctor page
public class SystemController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var query = new GetCitiesQuery();
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }
}
