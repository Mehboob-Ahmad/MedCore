using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Clinical;

public class AppointmentStatusHistory : SoftDeleteEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public string FromStatus { get; set; } = null!;
    public string ToStatus { get; set; } = null!;
    public Guid? ChangedByUserId { get; set; }
    public User? ChangedByUser { get; set; }
    public string? Reason { get; set; }
}
