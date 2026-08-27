using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class DoctorQualification : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;
    
    public string Degree { get; set; } = null!;
    public string Institution { get; set; } = null!;
    public int CompletionYear { get; set; }
}
