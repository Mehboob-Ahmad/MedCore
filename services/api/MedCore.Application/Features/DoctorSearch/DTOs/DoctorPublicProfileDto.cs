using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.DoctorSearch.DTOs;

public class DoctorPublicProfileDto
{
    public Guid DoctorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal ConsultationFee { get; set; }
    public int YearsOfExperience { get; set; }
    public string? CityName { get; set; }
    public string? Address { get; set; }
    public string? ClinicName { get; set; }
    public string? Languages { get; set; }
    public string? Gender { get; set; }
    public List<string> Specializations { get; set; } = new();
    
    // Simple availability summary or true/false if currently accepting appointments
    public bool IsAcceptingNewPatients { get; set; }
    public List<DoctorAvailabilityDto> Availabilities { get; set; } = new();
}

public class DoctorAvailabilityDto
{
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
