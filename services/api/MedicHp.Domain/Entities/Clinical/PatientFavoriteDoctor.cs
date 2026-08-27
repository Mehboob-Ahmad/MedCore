using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientFavoriteDoctor : SoftDeleteEntity
{
    public Guid PatientId { get; set; }
    public PatientProfile Patient { get; set; } = null!;
    
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; } = null!;
}
