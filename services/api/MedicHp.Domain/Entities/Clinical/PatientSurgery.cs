using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientSurgery : BaseEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    
    public string SurgeryName { get; set; } = null!;
    public DateTime? SurgeryDate { get; set; }
    public string? SurgeonName { get; set; }
    public string? HospitalName { get; set; }
    public string? Notes { get; set; }
}
