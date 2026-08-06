using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Consultations.DTOs;

public class CreateConsultationDto
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
    public string? ClinicalNotes { get; set; }
    public bool IsFinalized { get; set; }

    public List<CreatePrescriptionItemDto> PrescriptionItems { get; set; } = new();
}

public class CreatePrescriptionItemDto
{
    public string MedicationName { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Instructions { get; set; } = string.Empty;
}
