using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class EmergencyContact : SoftDeleteEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Relationship { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
}
