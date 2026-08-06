using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Doctors.Commands.ConfigureAvailability;
using MedCore.Application.Features.Doctors.Commands.UpdateDoctorProfile;
using MedCore.Application.Features.Doctors.Queries.GetAvailableSlots;
using MedCore.Application.Features.Doctors.Queries.GetDoctorDashboard;
using MedCore.Application.Features.Doctors.Queries.GetDoctorProfile;
using MedCore.Application.Features.Doctors.Queries.SearchDoctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/doctors")]
[Authorize] // Both patients and doctors can view doctor profiles
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var query = new GetDoctorProfileQuery(userId);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateDoctorProfileCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        command.UserId = userId;
        
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }

    [HttpPost("availability")]
    public async Task<IActionResult> ConfigureAvailability([FromBody] ConfigureAvailabilityCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        command.UserId = userId;
        
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var query = new GetDoctorDashboardQuery(userId);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchDoctors([FromQuery] string? searchTerm, [FromQuery] string? specialty, [FromQuery] string? gender)
    {
        var query = new SearchDoctorsQuery
        {
            SearchTerm = searchTerm,
            Specialty = specialty,
            Gender = gender
        };
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("{id}/slots")]
    public async Task<IActionResult> GetAvailableSlots(Guid id, [FromQuery] DateTime date)
    {
        var query = new GetAvailableSlotsQuery(id, date);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }
}
