using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Appointments.Commands.BookAppointment;
using MedCore.Application.Features.Appointments.Commands.CancelAppointment;
using MedCore.Application.Features.Appointments.Commands.RescheduleAppointment;
using MedCore.Application.Features.Appointments.Commands.UpdateAppointmentStatus;
using MedCore.Application.Features.Appointments.Queries.GetAppointmentDetails;
using MedCore.Application.Features.Appointments.Queries.GetDoctorAppointments;
using MedCore.Application.Features.Appointments.Queries.GetPatientAppointments;
using MedCore.Application.Features.Auth.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AppointmentsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    // ───────────── Patient Endpoints ─────────────

    /// <summary>
    /// Book a new appointment (Patient)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.UserId = userId.Value;
        var result = await _mediator.Send(command);

        return Created("", new { success = true, data = new { appointmentId = result } });
    }

    /// <summary>
    /// Get patient's appointments with filters (Patient)
    /// </summary>
    [HttpGet("patient")]
    public async Task<IActionResult> GetPatientAppointments(
        [FromQuery] string? filter,
        [FromQuery] string? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? doctorId)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetPatientAppointmentsQuery(userId.Value, filter)
        {
            Status = status,
            DateFrom = dateFrom,
            DateTo = dateTo,
            DoctorId = doctorId
        };
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    /// <summary>
    /// Cancel an appointment (Patient or Doctor)
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] CancelAppointmentCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.Id = id;
        command.UserId = userId.Value;
        await _mediator.Send(command);

        return Ok(new { success = true, message = "Appointment cancelled." });
    }

    /// <summary>
    /// Reschedule an appointment (Patient or Doctor)
    /// </summary>
    [HttpPost("{id}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(Guid id, [FromBody] RescheduleAppointmentCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.AppointmentId = id;
        command.UserId = userId.Value;
        await _mediator.Send(command);

        return Ok(new { success = true, message = "Appointment rescheduled." });
    }

    /// <summary>
    /// Get appointment details (Patient or Doctor)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentDetails(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetAppointmentDetailsQuery(id, userId.Value);
        var result = await _mediator.Send(query);

        if (result == null) return NotFound(new { success = false, message = "Appointment not found." });

        return Ok(new { success = true, data = result });
    }

    // ───────────── Doctor Endpoints ─────────────

    /// <summary>
    /// Get doctor's appointments / calendar data (Doctor)
    /// </summary>
    [HttpGet("doctor")]
    public async Task<IActionResult> GetDoctorAppointments(
        [FromQuery] string? filter,
        [FromQuery] string? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? patientId,
        [FromQuery] string? search)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetDoctorAppointmentsQuery(userId.Value, filter)
        {
            Status = status,
            DateFrom = dateFrom,
            DateTo = dateTo,
            PatientId = patientId,
            SearchTerm = search
        };
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    /// <summary>
    /// Update appointment status (Doctor): Confirm, Reject, Complete, NoShow
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromBody] UpdateAppointmentStatusCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.AppointmentId = id;
        command.DoctorId = userId.Value;
        await _mediator.Send(command);

        return Ok(new { success = true, message = $"Appointment status updated to '{command.Status}'." });
    }
}
