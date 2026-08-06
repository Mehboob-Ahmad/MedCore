using System;
using System.Collections.Generic;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;

namespace MedCore.Domain.Entities.Clinical;

public class Appointment : SoftDeleteEntity
{
    public Guid PatientId { get; set; }
    public User Patient { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = "Pending";
    public string? BookingNote { get; set; }
    public string? CancellationReason { get; set; }
    public string? DoctorNotes { get; set; }
    public DateTime? SuggestedNewTime { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    public ICollection<AppointmentStatusHistory> StatusHistory { get; set; } = new List<AppointmentStatusHistory>();
    public Consultation? Consultation { get; set; }
}
