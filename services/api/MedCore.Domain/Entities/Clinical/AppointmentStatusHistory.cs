using System;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;

namespace MedCore.Domain.Entities.Clinical;

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
