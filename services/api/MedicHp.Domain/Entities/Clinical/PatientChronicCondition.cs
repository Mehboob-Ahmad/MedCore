using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientChronicCondition : SoftDeleteEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    public string ConditionName { get; set; } = null!;
    public DateOnly? DiagnosedDate { get; set; }
    public string? Notes { get; set; }
}
