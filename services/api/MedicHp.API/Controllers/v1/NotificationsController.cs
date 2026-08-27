using System;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using MedicHp.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetNotificationsQuery(userId.Value);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var command = new MarkNotificationAsReadCommand { Id = id, UserId = userId.Value };
        await _mediator.Send(command);

        return Ok(new { success = true });
    }
}
