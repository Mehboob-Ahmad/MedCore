using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class DoctorFavoriteMedicine : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; } = null!;
    
    public string MedicationName { get; set; } = null!;
    
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
