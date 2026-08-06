using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Appointments.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    
    // Doctor info
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = null!;
    public string? DoctorProfilePhotoUrl { get; set; }
    public string? Specialty { get; set; }
    public string? ClinicName { get; set; }
    public string? ClinicAddress { get; set; }
    
    // Patient info
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    
    // Schedule
    public DateTime ScheduledAt { get; set; }
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public int DurationMinutes { get; set; }
    
    // Status
    public string Status { get; set; } = null!;
    public string StatusColor { get; set; } = null!;
    
    // Details
    public string? BookingNote { get; set; }
    public string? CancellationReason { get; set; }
    public string? DoctorNotes { get; set; }
    public decimal? ConsultationFee { get; set; }
    public DateTime? SuggestedNewTime { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AppointmentDetailDto : AppointmentDto
{
    public List<AppointmentStatusHistoryDto> StatusTimeline { get; set; } = new();
}

public class AppointmentStatusHistoryDto
{
    public string FromStatus { get; set; } = null!;
    public string ToStatus { get; set; } = null!;
    public string? ChangedByName { get; set; }
    public string? Reason { get; set; }
    public DateTime ChangedAt { get; set; }
}

public class AvailableDateDto
{
    public DateOnly Date { get; set; }
    public int AvailableSlotCount { get; set; }
}
