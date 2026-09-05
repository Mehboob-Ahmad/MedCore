using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Auth.DTOs;

public class RegisterDoctorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public Guid DegreeFileId { get; set; }
    public Guid? LicenseFileId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }
}
