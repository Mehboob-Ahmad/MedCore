using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Lookup;

namespace MedicHp.Domain.Entities.Clinical;

public class DoctorSpecialization : SoftDeleteEntity
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
}
