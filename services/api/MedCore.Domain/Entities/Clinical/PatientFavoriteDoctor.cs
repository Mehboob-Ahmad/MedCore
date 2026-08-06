using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class PatientFavoriteDoctor : SoftDeleteEntity
{
    public Guid PatientId { get; set; }
    public PatientProfile Patient { get; set; } = null!;
    
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; } = null!;
}
