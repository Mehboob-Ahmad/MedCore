using System;
using System.Threading.Tasks;
using MedicHp.Application.Features.Admin.Queries.GetSystemStats;
using MedicHp.Application.Features.Auth.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "SystemAdmin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AdminController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get system-wide statistics for the admin dashboard
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetSystemStats()
    {
        var query = new GetSystemStatsQuery();
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] MedicHp.Application.Features.Admin.Queries.GetUsers.GetUsersQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPost("users/{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(Guid id, [FromBody] MedicHp.Application.Features.Admin.Commands.ToggleUserStatus.ToggleUserStatusCommand command)
    {
        command.UserId = id;
        var result = await _mediator.Send(command);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("demo-requests")]
    public async Task<IActionResult> GetDemoRequests([FromQuery] MedicHp.Application.Features.Admin.Queries.GetDemoRequests.GetDemoRequestsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("demo-requests/{id}/status")]
    public async Task<IActionResult> UpdateDemoRequestStatus(Guid id, [FromBody] MedicHp.Application.Features.Admin.Commands.UpdateDemoRequestStatus.UpdateDemoRequestStatusCommand command)
    {
        command.RequestId = id;
        var result = await _mediator.Send(command);
        return Ok(new { success = true, data = result });
    }
    [HttpPost("demo-requests/{id}/create-doctor")]
    public async Task<IActionResult> CreateDoctorFromDemo(Guid id)
    {
        var command = new MedicHp.Application.Features.Admin.Commands.CreateDoctorFromDemo.CreateDoctorFromDemoCommand { RequestId = id };
        var result = await _mediator.Send(command);
        return Ok(new { success = true, data = result });
    }
}
