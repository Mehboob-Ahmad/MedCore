using System;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Features.AI.Commands.AskAi;
using MedicHp.Application.Features.AI.Queries.GetAiChatHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/ai")]
[Authorize] // Require users to be logged in to ask the AI
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;

    public AIController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var query = new GetAiChatHistoryQuery { UserId = userId };
        
        var history = await _mediator.Send(query);
        return Ok(new { success = true, data = history });
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskAiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new { success = false, message = "Prompt cannot be empty." });
        }

        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        var command = new AskAiCommand 
        { 
            UserId = userId, 
            Prompt = request.Prompt 
        };

        var response = await _mediator.Send(command);

        return Ok(new { success = true, data = new { answer = response } });
    }
}

public class AskAiRequest
{
    public string Prompt { get; set; } = string.Empty;
}
