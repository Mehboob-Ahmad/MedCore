using System;
using System.Text.Json.Serialization;
using MediatR;

namespace MedCore.Application.Features.Doctors.Commands.UpdateDoctorProfile;

public class UpdateDoctorProfileCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    
    public string Bio { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public int ExperienceYears { get; set; }
    
    // In Phase 1, we only handle basic professional details.
    // Specialties and more complex collections can be updated in separate endpoints if needed.
}
