using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Doctors.DTOs;

public class DoctorSearchDto
{
    public Guid DoctorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public int ExperienceYears { get; set; }
    public string Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    
    public string? ProfessionalType { get; set; }
    public List<string> Specializations { get; set; } = new();
    
    // Rating/Reviews mocked or calculated
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
}

public class DoctorSlotDto
{
    public Guid SlotId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
