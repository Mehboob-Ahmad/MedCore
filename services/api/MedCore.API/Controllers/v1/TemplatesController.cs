using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Templates.Commands.CreateConsultationTemplate;
using MedCore.Application.Features.Templates.Commands.CreatePrescriptionTemplate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/templates")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("consultations")]
    public async Task<IActionResult> CreateConsultationTemplate([FromBody] CreateConsultationTemplateCommand command)
    {
        var templateId = await _mediator.Send(command);
        return Created($"/api/v1/templates/consultations/{templateId}", new { Id = templateId });
    }

    [HttpPost("prescriptions")]
    public async Task<IActionResult> CreatePrescriptionTemplate([FromBody] CreatePrescriptionTemplateCommand command)
    {
        var templateId = await _mediator.Send(command);
        return Created($"/api/v1/templates/prescriptions/{templateId}", new { Id = templateId });
    }
}
