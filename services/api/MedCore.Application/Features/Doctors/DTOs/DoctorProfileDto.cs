using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Doctors.DTOs;

public class DoctorProfileDto
{
    public Guid Id { get; set; }
    
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfilePhotoUrl { get; set; } = string.Empty;
    
    // Professional Information
    public string MedicalLicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Bio { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public List<string> Specializations { get; set; } = new();
    public List<string> Qualifications { get; set; } = new();
    public List<string> Languages { get; set; } = new();

    // Clinic Information
    public string ClinicName { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
    public string ClinicCity { get; set; } = string.Empty;
    public string ClinicPhoneNumber { get; set; } = string.Empty;
    public string GoogleMapsUrl { get; set; } = string.Empty;

    public List<DoctorAvailabilityDto> Availabilities { get; set; } = new();
}

public class DoctorAvailabilityDto
{
    public short DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
