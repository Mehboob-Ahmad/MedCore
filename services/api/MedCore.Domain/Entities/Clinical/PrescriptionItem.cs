using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class PrescriptionItem : SoftDeleteEntity
{
    public Guid PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;
    public string MedicationName { get; set; } = null!;
    public string? Strength { get; set; } // e.g. 500mg
    public string Dosage { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string Duration { get; set; } = null!;
    public string? Route { get; set; } // e.g. Oral, Topical
    public string? Timing { get; set; } // e.g. After meals
    public string? Quantity { get; set; } // e.g. 10 tablets
    public string? Instructions { get; set; }
    public int SortOrder { get; set; } = 0;
}
