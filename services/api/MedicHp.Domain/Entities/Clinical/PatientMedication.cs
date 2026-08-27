using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientMedication : SoftDeleteEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    public string MedicationName { get; set; } = null!;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? PrescribingDoctor { get; set; }
    public bool IsCurrent { get; set; } = true;
}
