using System;
using MediatR;

namespace MedicHp.Application.Features.Public.Commands.SubmitDemoRequest;

public class SubmitDemoRequestCommand : IRequest<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ClinicOrHospital { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string ProfessionalQualification { get; set; } = string.Empty;
    public string? AdditionalInformation { get; set; }
    public string? DegreeImageUrl { get; set; }
    public string? LicenseImageUrl { get; set; }
}
