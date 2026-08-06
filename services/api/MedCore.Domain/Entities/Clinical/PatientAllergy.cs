using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class PatientAllergy : SoftDeleteEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    public string AllergyName { get; set; } = null!;
    public string? Severity { get; set; }
    public string? Notes { get; set; }
}
