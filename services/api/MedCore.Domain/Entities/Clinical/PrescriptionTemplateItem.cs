using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class PrescriptionTemplateItem : BaseEntity
{
    public Guid PrescriptionTemplateId { get; set; }
    public PrescriptionTemplate PrescriptionTemplate { get; set; } = null!;
    
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
