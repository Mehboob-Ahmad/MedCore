using System.Collections.Generic;
using MediatR;
using System;

namespace MedCore.Application.Features.Templates.Commands.CreatePrescriptionTemplate;

public class CreatePrescriptionTemplateCommand : IRequest<Guid>
{
    public string TemplateName { get; set; } = null!;
    public string? Notes { get; set; }
    
    public List<PrescriptionTemplateItemDto> Items { get; set; } = new();
}

public class PrescriptionTemplateItemDto
{
    public string MedicationName { get; set; } = null!;
    public string? Strength { get; set; }
    public string Dosage { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string? Duration { get; set; }
    public string? Route { get; set; }
    public string? Timing { get; set; }
    public string? Quantity { get; set; }
    public string? Instructions { get; set; }
}
