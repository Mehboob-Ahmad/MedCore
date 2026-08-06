using System;
using System.Collections.Generic;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;
using MedCore.Domain.Entities.Lookup;

namespace MedCore.Domain.Entities.Clinical;

public class DoctorProfile : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public decimal ConsultationFee { get; set; }
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public Guid? CityId { get; set; }
    public City? City { get; set; }
    public string? Address { get; set; }
    public string? ClinicName { get; set; }
    public string? Languages { get; set; }
    public string VerificationStatus { get; set; } = "Pending";
    public int SlotDurationMinutes { get; set; } = 30;

    public ICollection<DoctorSpecialization> Specializations { get; set; } = new List<DoctorSpecialization>();
    public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
    public ICollection<DoctorUnavailability> Unavailabilities { get; set; } = new List<DoctorUnavailability>();
}
