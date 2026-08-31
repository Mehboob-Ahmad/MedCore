using System;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Application.Features.Patients.Commands.UpdatePatientProfile;
using MedicHp.Application.Features.Patients.Queries.GetPatientProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/patients")]
[Authorize(Roles = "Patient,SystemAdmin")] // Adjust as necessary
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public PatientsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("search")]
    [Authorize(Roles = "Doctor,SystemAdmin")] // Only doctors/admins can search all patients
    public async Task<IActionResult> SearchPatients([FromQuery] string? searchTerm)
    {
        var query = new MedicHp.Application.Features.Patients.Queries.SearchMedicHpPatients.SearchMedicHpPatientsQuery(searchTerm);
        var result = await _mediator.Send(query);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{patientId}/summary")]
    [Authorize(Roles = "Doctor,SystemAdmin")]
    public async Task<IActionResult> GetDoctorPatientSummary(Guid patientId)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        var query = new MedicHp.Application.Features.Patients.Queries.GetDoctorPatientSummary.GetDoctorPatientSummaryQuery(doctorId.Value, patientId);
        var result = await _mediator.Send(query);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{patientId}/clinical-summary")]
    [Authorize(Roles = "Doctor,SystemAdmin")]
    public async Task<IActionResult> GetPatientClinicalSummary(Guid patientId)
    {
        var doctorId = _currentUserService.UserId;
        if (doctorId == null) return Unauthorized();

        var query = new MedicHp.Application.Features.Consultations.Queries.GetPatientSummary.GetPatientSummaryQuery 
        { 
            PatientId = patientId,
            DoctorId = doctorId.Value
        };
        var result = await _mediator.Send(query);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetPatientProfileQuery(userId.Value);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.UserId = userId.Value; // Ensure the user can only update their own profile
        await _mediator.Send(command);

        // Fetch updated profile completion percentage (mocked via another query or command result)
        // For simplicity, we just return success
        return Ok(new { success = true, message = "Profile updated successfully." });
    }

    // Emergency Contacts

    [HttpPost("emergency-contacts")]
    public async Task<IActionResult> AddEmergencyContact([FromBody] MedicHp.Application.Features.Patients.Commands.EmergencyContacts.AddEmergencyContactCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.UserId = userId.Value;
        var result = await _mediator.Send(command);
        return Created("", new { success = true, data = result });
    }

    [HttpPut("emergency-contacts/{id}")]
    public async Task<IActionResult> UpdateEmergencyContact(Guid id, [FromBody] MedicHp.Application.Features.Patients.Commands.EmergencyContacts.UpdateEmergencyContactCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.Id = id;
        command.UserId = userId.Value;
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    [HttpDelete("emergency-contacts/{id}")]
    public async Task<IActionResult> DeleteEmergencyContact(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        var command = new MedicHp.Application.Features.Patients.Commands.EmergencyContacts.DeleteEmergencyContactCommand { Id = id, UserId = userId.Value };
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    // Allergies

    [HttpPost("allergies")]
    public async Task<IActionResult> AddAllergy([FromBody] MedicHp.Application.Features.Patients.Commands.Allergies.AddAllergyCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.UserId = userId.Value;
        var result = await _mediator.Send(command);
        return Created("", new { success = true, data = result });
    }

    [HttpPut("allergies/{id}")]
    public async Task<IActionResult> UpdateAllergy(Guid id, [FromBody] MedicHp.Application.Features.Patients.Commands.Allergies.UpdateAllergyCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.Id = id;
        command.UserId = userId.Value;
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    [HttpDelete("allergies/{id}")]
    public async Task<IActionResult> DeleteAllergy(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        var command = new MedicHp.Application.Features.Patients.Commands.Allergies.DeleteAllergyCommand { Id = id, UserId = userId.Value };
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    // Chronic Conditions

    [HttpPost("chronic-conditions")]
    public async Task<IActionResult> AddChronicCondition([FromBody] MedicHp.Application.Features.Patients.Commands.ChronicConditions.AddChronicConditionCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.UserId = userId.Value;
        var result = await _mediator.Send(command);
        return Created("", new { success = true, data = result });
    }

    [HttpPut("chronic-conditions/{id}")]
    public async Task<IActionResult> UpdateChronicCondition(Guid id, [FromBody] MedicHp.Application.Features.Patients.Commands.ChronicConditions.UpdateChronicConditionCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.Id = id;
        command.UserId = userId.Value;
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    [HttpDelete("chronic-conditions/{id}")]
    public async Task<IActionResult> DeleteChronicCondition(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        var command = new MedicHp.Application.Features.Patients.Commands.ChronicConditions.DeleteChronicConditionCommand { Id = id, UserId = userId.Value };
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    // Medications

    [HttpPost("medications")]
    public async Task<IActionResult> AddMedication([FromBody] MedicHp.Application.Features.Patients.Commands.Medications.AddMedicationCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.UserId = userId.Value;
        var result = await _mediator.Send(command);
        return Created("", new { success = true, data = result });
    }

    [HttpPut("medications/{id}")]
    public async Task<IActionResult> UpdateMedication(Guid id, [FromBody] MedicHp.Application.Features.Patients.Commands.Medications.UpdateMedicationCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        command.Id = id;
        command.UserId = userId.Value;
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    [HttpDelete("medications/{id}")]
    public async Task<IActionResult> DeleteMedication(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();
        
        var command = new MedicHp.Application.Features.Patients.Commands.Medications.DeleteMedicationCommand { Id = id, UserId = userId.Value };
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    // Dashboard

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new MedicHp.Application.Features.Patients.Queries.GetPatientDashboard.GetPatientDashboardQuery(userId.Value);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }
}
