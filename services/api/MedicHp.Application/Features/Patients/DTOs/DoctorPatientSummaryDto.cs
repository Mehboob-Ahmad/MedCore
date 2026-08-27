using System;
using System.Collections.Generic;
using MedicHp.Application.Features.Records.DTOs;

namespace MedicHp.Application.Features.Patients.DTOs;

public class DoctorPatientSummaryDto
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;

    public List<AllergyDto> Allergies { get; set; } = new();
    public List<ChronicConditionDto> ChronicConditions { get; set; } = new();
    public List<MedicationDto> Medications { get; set; } = new();

    // From the doctor's perspective
    public int TotalConsultations { get; set; }
    public DateTime? LastConsultationDate { get; set; }
}

public class PatientSearchDto
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
