using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Application.Features.Chat.Commands.SendMessage;
using MedCore.Application.Features.Chat.Queries.GetConversations;
using MedCore.Application.Features.Chat.Queries.GetMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ChatController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetConversationsQuery(userId.Value);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("conversations/{id}/messages")]
    public async Task<IActionResult> GetMessages(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetMessagesQuery(userId.Value, id);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.ConversationId = id;
        command.UserId = userId.Value;
        
        var result = await _mediator.Send(command);

        return Created("", new { success = true, data = result });
    }
}
