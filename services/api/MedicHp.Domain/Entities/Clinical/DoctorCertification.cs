using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class DoctorCertification : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public string IssuingOrganization { get; set; } = null!;
    public int? Year { get; set; }
}
