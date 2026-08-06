using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Auth.DTOs;

public class RegisterDoctorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public List<Guid> SpecializationIds { get; set; } = new();
    public int YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseAuthority { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }
}
