using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Admin;

public class DemoRequest : AuditableEntity
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public string City { get; set; } = null!;
    public string ClinicOrHospital { get; set; } = null!;
    public int YearsOfExperience { get; set; }
    public string ProfessionalQualification { get; set; } = null!;
    public string? AdditionalInformation { get; set; }
    public string? DegreeImageUrl { get; set; }
    public string? LicenseImageUrl { get; set; }
    
    public DemoRequestStatus Status { get; set; } = DemoRequestStatus.Pending;
    public string? Notes { get; set; }
}

public enum DemoRequestStatus
{
    Pending,
    UnderReview,
    Approved,
    Rejected,
    DemoCreated,
    ConvertedToProduction
}
