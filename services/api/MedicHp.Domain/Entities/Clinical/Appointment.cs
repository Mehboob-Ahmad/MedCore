using System;
using System.Collections.Generic;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Clinical;

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
    
    // Payment tracking
    public string PaymentStatus { get; set; } = "Pending";
    public DateTime? PaymentConfirmedAt { get; set; }
    public Guid? PaymentConfirmedByUserId { get; set; }
    
    // Reminder tracking (simple approach — avoids separate table)
    public DateTime? AppointmentReminderSentAt { get; set; }
    public DateTime? PaymentReminderSentAt { get; set; }
    public DateTime? PaymentOverdueNotifiedAt { get; set; }
    
    public ICollection<AppointmentStatusHistory> StatusHistory { get; set; } = new List<AppointmentStatusHistory>();
    public Consultation? Consultation { get; set; }
}
