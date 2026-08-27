using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Doctors.Commands.UpdateDoctorProfile;

public class UpdateDoctorProfileCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    
    public string ProfessionalType { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string RegulatoryBody { get; set; } = string.Empty;
    
    public string Bio { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public int ExperienceYears { get; set; }
    
    public string WhatsAppNumber { get; set; } = string.Empty;
    public bool WhatsAppEnabled { get; set; }

    public List<DoctorQualificationDto> Qualifications { get; set; } = new();
    public List<DoctorCertificationDto> Certifications { get; set; } = new();
}
