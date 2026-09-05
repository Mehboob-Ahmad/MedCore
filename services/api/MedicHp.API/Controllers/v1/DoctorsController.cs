using System;
using System.Threading.Tasks;
using MedicHp.Application.Features.Doctors.Commands.CompleteProfile;
using MedicHp.Application.Features.Doctors.Commands.ConfigureAvailability;
using MedicHp.Application.Features.Doctors.Commands.UpdateDoctorProfile;
using MedicHp.Application.Features.Doctors.Queries.GetAvailableSlots;
using MedicHp.Application.Features.Doctors.Queries.GetDoctorDashboard;
using MedicHp.Application.Features.Doctors.Queries.GetDoctorProfile;
using MedicHp.Application.Features.DoctorSearch.Queries.SearchDoctors;
using MedicHp.Application.Features.Doctors.Commands.AddPatient;
using MedicHp.Application.Features.DoctorSearch.Queries.GetPublicDoctorProfile;
using MedicHp.Application.Features.DoctorSearch.Queries.GetRelatedDoctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

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

    [HttpPut("profile/complete")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteDoctorProfileCommand command)
    {
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

    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var query = new MedicHp.Application.Features.Doctors.Queries.GetDoctorPaymentMethods.GetDoctorPaymentMethodsQuery(userId);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPut("payment-methods")]
    public async Task<IActionResult> ConfigurePaymentMethods([FromBody] MedicHp.Application.Features.Doctors.Commands.ConfigurePaymentMethods.ConfigurePaymentMethodsCommand command)
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

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchDoctors([FromQuery] string? searchTerm, [FromQuery] string? specialty, [FromQuery] string? gender, [FromQuery] System.Collections.Generic.List<Guid>? cityIds)
    {
        var query = new SearchDoctorsQuery
        {
            SearchTerm = searchTerm,
            Specialization = specialty, // Note: The previous code mapped specialty to Specialization. I should use Specialization = specialty
            Gender = gender,
            CityIds = cityIds
        };
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPublicProfile(Guid id)
    {
        var query = new GetPublicDoctorProfileQuery { DoctorId = id };
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [AllowAnonymous]
    [HttpGet("{id}/related")]
    public async Task<IActionResult> GetRelatedDoctors(Guid id, [FromQuery] int limit = 4)
    {
        var query = new GetRelatedDoctorsQuery { DoctorId = id, Limit = limit };
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [AllowAnonymous]
    [HttpGet("{id}/slots")]
    public async Task<IActionResult> GetAvailableSlots(Guid id, [FromQuery] DateTime date)
    {
        var query = new GetAvailableSlotsQuery(id, date);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPost("patients")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> AddPatient([FromBody] AddPatientCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        command.DoctorId = userId;
        
        var result = await _mediator.Send(command);
        return Created("", new { success = true, message = "Patient added successfully. An email has been sent to them.", data = result });
    }
}
