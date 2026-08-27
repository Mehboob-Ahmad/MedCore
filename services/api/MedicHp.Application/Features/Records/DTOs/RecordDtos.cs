using System;

namespace MedicHp.Application.Features.Records.DTOs;

public class TimelineEventDto
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } // "Consultation", "Prescription", "LabResult"
    public DateTime EventDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? DoctorName { get; set; }
    
    // Links to the actual documents if needed
    public Guid? ReferenceId { get; set; }
}

public class ConsultationSummaryDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string? DoctorName { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
}

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public DateTime IssueDate { get; set; }
    public string? DoctorName { get; set; }
    public string? MedicationName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
}
