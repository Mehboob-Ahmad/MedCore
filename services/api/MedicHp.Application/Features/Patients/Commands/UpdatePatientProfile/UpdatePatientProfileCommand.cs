using MediatR;
using System;

namespace MedicHp.Application.Features.Patients.Commands.UpdatePatientProfile;

public class UpdatePatientProfileCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public Guid? CityId { get; set; }
    public string? Address { get; set; }
    public bool? DataSharingConsent { get; set; }
    
    public string? FamilyMedicalHistory { get; set; }
    public string? MedicalHistory { get; set; }
    public string? ImmunizationHistory { get; set; }
    public string? LifestyleInformation { get; set; }

    public List<MedicHp.Application.Features.Patients.DTOs.SurgeryDto> Surgeries { get; set; } = new();
    public List<MedicHp.Application.Features.Patients.DTOs.HospitalizationDto> Hospitalizations { get; set; } = new();
}
