using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Favorites.Commands.AddFavoriteMedicine;
using MedCore.Application.Features.Favorites.Commands.RemoveFavoriteMedicine;
using MedCore.Application.Features.Favorites.Queries.GetFavoriteMedicines;
using MedCore.Application.Features.Productivity.Commands.CopyPreviousPrescription;
using MedCore.Application.Features.Productivity.Queries.GetClinicalReminders;
using MedCore.Application.Features.Productivity.Queries.GetDoctorAnalytics;
using MedCore.Application.Features.Productivity.Queries.GetDoctorDrafts;
using MedCore.Application.Features.Productivity.Queries.GetFollowUpManagerData;
using MedCore.Application.Features.Productivity.Queries.GetRecentMedicines;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/productivity")]
[Authorize]
public class ProductivityController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductivityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var result = await _mediator.Send(new GetDoctorAnalyticsQuery());
        return Ok(result);
    }

    [HttpGet("reminders")]
    public async Task<IActionResult> GetReminders()
    {
        var result = await _mediator.Send(new GetClinicalRemindersQuery());
        return Ok(result);
    }

    [HttpGet("follow-ups")]
    public async Task<IActionResult> GetFollowUpManagerData()
    {
        var result = await _mediator.Send(new GetFollowUpManagerDataQuery());
        return Ok(result);
    }

    [HttpGet("drafts")]
    public async Task<IActionResult> GetDrafts()
    {
        var result = await _mediator.Send(new GetDoctorDraftsQuery());
        return Ok(result);
    }

    [HttpPost("copy-prescription")]
    public async Task<IActionResult> CopyPrescription([FromBody] CopyPreviousPrescriptionCommand command)
    {
        var success = await _mediator.Send(command);
        return success ? Ok() : BadRequest();
    }

    [HttpGet("medicines/favorites")]
    public async Task<IActionResult> GetFavoriteMedicines()
    {
        var result = await _mediator.Send(new GetFavoriteMedicinesQuery());
        return Ok(result);
    }

    [HttpPost("medicines/favorites")]
    public async Task<IActionResult> AddFavoriteMedicine([FromBody] AddFavoriteMedicineCommand command)
    {
        var id = await _mediator.Send(command);
        return Created("", new { Id = id });
    }

    [HttpDelete("medicines/favorites/{medicationName}")]
    public async Task<IActionResult> RemoveFavoriteMedicine(string medicationName)
    {
        var success = await _mediator.Send(new RemoveFavoriteMedicineCommand { MedicationName = medicationName });
        return success ? NoContent() : NotFound();
    }

    [HttpGet("medicines/recent")]
    public async Task<IActionResult> GetRecentMedicines([FromQuery] int limit = 10)
    {
        var result = await _mediator.Send(new GetRecentMedicinesQuery { Limit = limit });
        return Ok(result);
    }
}
