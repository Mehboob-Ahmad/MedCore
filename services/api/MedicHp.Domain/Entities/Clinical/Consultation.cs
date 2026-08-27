using System;
using System.Collections.Generic;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Clinical;

public class Consultation : SoftDeleteEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;
    public Guid PatientId { get; set; }
    public User Patient { get; set; } = null!;
    public string ChiefComplaint { get; set; } = null!;
    public string Symptoms { get; set; } = null!;
    public string Diagnosis { get; set; } = null!;
    public string TreatmentPlan { get; set; } = null!;
    public string? ClinicalNotes { get; set; }
    
    // New Fields for Consultation Workflow
    public string? VisitType { get; set; } // e.g., Initial, Follow-up, Routine
    public string? PrivateNotes { get; set; } // Doctor's eyes only
    public string? PatientNotes { get; set; } // Instructions for patient
    
    // Follow-up
    public DateTime? FollowUpDate { get; set; }
    public string? FollowUpUrgency { get; set; } // e.g., Normal, Urgent
    public string? FollowUpInstructions { get; set; }

    public bool IsFinalized { get; set; }
    public DateTime? FinalizedAt { get; set; }
    
    public ConsultationVital? Vitals { get; set; }
    public ICollection<ConsultationAddendum> Addenda { get; set; } = new List<ConsultationAddendum>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
