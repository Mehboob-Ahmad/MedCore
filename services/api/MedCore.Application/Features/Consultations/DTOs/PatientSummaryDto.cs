using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Consultations.DTOs;

public class PatientSummaryDto
{
    public Guid PatientId { get; set; }
    public string FullName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public string BloodGroup { get; set; } = null!;
    public string? ProfilePhotoUrl { get; set; }

    public EmergencyContactDto? EmergencyContact { get; set; }
    
    public List<string> KnownAllergies { get; set; } = new();
    public List<string> ChronicConditions { get; set; } = new();
    public List<string> CurrentMedications { get; set; } = new();

    public DateTime? LastConsultationDate { get; set; }
    public string? LastConsultationDiagnosis { get; set; }
    
    public DateTime? UpcomingFollowUpDate { get; set; }
}

public class EmergencyContactDto
{
    public string Name { get; set; } = null!;
    public string Relationship { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}
