using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Features.Public.Commands.SubmitDemoRequest;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/public")]
public class PublicController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("demo-requests")]
    public async Task<IActionResult> SubmitDemoRequest([FromBody] SubmitDemoRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = true, data = result });
    }
}
