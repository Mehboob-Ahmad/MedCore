using System;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Lookup;

namespace MedCore.Domain.Entities.Clinical;

public class DoctorSpecialization : SoftDeleteEntity
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
}
