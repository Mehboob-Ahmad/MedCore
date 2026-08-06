using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Patients.DTOs;

public class PatientProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public CityDto? City { get; set; }
    public string? Address { get; set; }
    public bool DataSharingConsent { get; set; }
    public int ProfileCompletionPct { get; set; }

    public List<EmergencyContactDto> EmergencyContacts { get; set; } = new();
    public List<AllergyDto> Allergies { get; set; } = new();
    public List<ChronicConditionDto> ChronicConditions { get; set; } = new();
    public List<MedicationDto> CurrentMedications { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
}

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class EmergencyContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Relationship { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsPrimary { get; set; }
}

public class AllergyDto
{
    public Guid Id { get; set; }
    public string AllergyName { get; set; }
    public string? Severity { get; set; }
    public string? Notes { get; set; }
}

public class ChronicConditionDto
{
    public Guid Id { get; set; }
    public string ConditionName { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public string? Notes { get; set; }
}

public class MedicationDto
{
    public Guid Id { get; set; }
    public string MedicationName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
}
