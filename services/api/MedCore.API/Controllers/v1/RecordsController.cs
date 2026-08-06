using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Application.Features.Records.Queries.GetConsultationSummary;
using MedCore.Application.Features.Records.Queries.GetPrescription;
using MedCore.Application.Features.Records.Queries.GetTimeline;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/records")]
[Authorize]
public class RecordsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public RecordsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("timeline")]
    public async Task<IActionResult> GetTimeline()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetTimelineQuery(userId.Value);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("consultations/{id}")]
    public async Task<IActionResult> GetConsultationSummary(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetConsultationSummaryQuery(userId.Value, id);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("prescriptions/{id}")]
    public async Task<IActionResult> GetPrescription(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetPrescriptionQuery(userId.Value, id);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }
}
