using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class DoctorAvailability : SoftDeleteEntity
{
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; } = null!;
    public short DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
