using System;
using System.Collections.Generic;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Lookup;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientProfile : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public int? Age => DateOfBirth.HasValue ? (DateTime.Today.Year - DateOfBirth.Value.Year - (DateOfBirth.Value.DayOfYear > DateTime.Today.DayOfYear ? 1 : 0)) : null;
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public Guid? CityId { get; set; }
    public City? City { get; set; }
    public string? Address { get; set; }
    public bool DataSharingConsent { get; set; } = true;
    public int ProfileCompletionPct { get; set; } = 0;

    public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    public ICollection<PatientAllergy> Allergies { get; set; } = new List<PatientAllergy>();
    public ICollection<PatientChronicCondition> ChronicConditions { get; set; } = new List<PatientChronicCondition>();
    public ICollection<PatientMedication> Medications { get; set; } = new List<PatientMedication>();
    public ICollection<PatientSurgery> Surgeries { get; set; } = new List<PatientSurgery>();
    public ICollection<PatientHospitalization> Hospitalizations { get; set; } = new List<PatientHospitalization>();
    public ICollection<PatientMedicalReport> MedicalReports { get; set; } = new List<PatientMedicalReport>();

    public string? FamilyMedicalHistory { get; set; }
    public string? MedicalHistory { get; set; }
    public string? ImmunizationHistory { get; set; }
    public string? LifestyleInformation { get; set; }
}
