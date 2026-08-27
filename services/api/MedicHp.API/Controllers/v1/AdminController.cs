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
}
