using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Application.Features.Consultations.Commands.FinalizeConsultation;
using MedCore.Application.Features.Consultations.Commands.SaveConsultationDraft;
using MedCore.Application.Features.Consultations.Commands.SavePrescription;
using MedCore.Application.Features.Consultations.Commands.StartConsultation;
using MedCore.Application.Features.Consultations.Queries.GetConsultationDetails;
using MedCore.Application.Features.Consultations.Queries.GetPatientConsultationHistory;
using MedCore.Application.Features.Consultations.Queries.SearchConsultations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/consultations")]
[Authorize]
public class ConsultationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ConsultationsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpPost("start")]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    public async Task<IActionResult> StartConsultation([FromBody] StartConsultationCommand command)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        command.DoctorId = doctorId.Value;
        var consultationId = await _mediator.Send(command);

        return Created("", new { success = true, data = new { consultationId } });
    }

    [HttpPut("{id}/draft")]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    public async Task<IActionResult> SaveConsultationDraft(Guid id, [FromBody] SaveConsultationDraftCommand command)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        command.ConsultationId = id;
        command.DoctorId = doctorId.Value;
        await _mediator.Send(command);

        return Ok(new { success = true, message = "Draft saved." });
    }

    [HttpPut("{id}/prescription")]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    public async Task<IActionResult> SavePrescription(Guid id, [FromBody] SavePrescriptionCommand command)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        command.ConsultationId = id;
        command.DoctorId = doctorId.Value;
        var prescriptionId = await _mediator.Send(command);

        return Ok(new { success = true, data = new { prescriptionId } });
    }

    [HttpPost("{id}/finalize")]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    public async Task<IActionResult> FinalizeConsultation(Guid id)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        var command = new FinalizeConsultationCommand
        {
            ConsultationId = id,
            DoctorId = doctorId.Value
        };
        await _mediator.Send(command);

        return Ok(new { success = true, message = "Consultation finalized." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultationDetails(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var isDoctor = User.IsInRole("Doctor") || User.IsInRole("SuperAdmin");

        var query = new GetConsultationDetailsQuery
        {
            ConsultationId = id,
            UserId = userId.Value,
            IsDoctor = isDoctor
        };
        
        var result = await _mediator.Send(query);
        if (result == null) return NotFound();

        return Ok(new { success = true, data = result });
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientConsultationHistory(Guid patientId)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetPatientConsultationHistoryQuery
        {
            PatientId = patientId,
            UserId = userId.Value
        };
        
        var result = await _mediator.Send(query);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("search")]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    public async Task<IActionResult> SearchConsultations([FromQuery] string? q, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        var query = new SearchConsultationsQuery
        {
            DoctorId = doctorId.Value,
            Query = q,
            DateFrom = dateFrom,
            DateTo = dateTo
        };
        
        var result = await _mediator.Send(query);
        return Ok(new { success = true, data = result });
    }
}
