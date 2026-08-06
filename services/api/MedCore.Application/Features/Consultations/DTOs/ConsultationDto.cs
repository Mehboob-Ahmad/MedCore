using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Consultations.DTOs;

public class ConsultationDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime Date { get; set; }
    
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
    
    // Notes
    public string? ClinicalNotes { get; set; }
    public string? PrivateNotes { get; set; }
    public string? PatientNotes { get; set; }
    
    // Follow up
    public string? VisitType { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? FollowUpUrgency { get; set; }
    public string? FollowUpInstructions { get; set; }

    public bool IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
}

public class ConsultationSummaryDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string? VisitType { get; set; }
    public bool IsFinalized { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public DateTime IssuedAt { get; set; }
    public string? Notes { get; set; }
    public List<PrescriptionItemDto> Items { get; set; } = new();
}

public class PrescriptionItemDto
{
    public Guid Id { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Strength { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? Timing { get; set; }
    public string? Quantity { get; set; }
    public string? Instructions { get; set; }
    public int SortOrder { get; set; }
}
