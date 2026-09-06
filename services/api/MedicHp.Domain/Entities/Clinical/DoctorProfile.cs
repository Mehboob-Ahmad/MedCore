using System;
using System.Collections.Generic;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Lookup;

namespace MedicHp.Domain.Entities.Clinical;

public class DoctorProfile : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string ProfessionalType { get; set; } = "Medical Doctor"; // e.g., Medical Doctor, Dentist, Psychologist
    
    // Demo Account logic
    public bool IsDemoAccount { get; set; } = false;
    public bool IsPaymentMocked => IsDemoAccount;

    // Registration & Verification
    public string? RegistrationNumber { get; set; }
    public string? Specialization { get; set; } // Doctor's typed specialization
    public string? RegulatoryBody { get; set; } // e.g., PM&DC, AHPC
    public string VerificationStatus { get; set; } = "Pending";
    public DateTime? VerificationDate { get; set; }
    public string? VerificationNotes { get; set; }
    public string? VerificationDocumentUrl { get; set; }

    // Professional Details
    public decimal ConsultationFee { get; set; }
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public Guid? CityId { get; set; }
    public City? City { get; set; }
    public string? Address { get; set; }
    public string? ClinicName { get; set; }
    public string? Languages { get; set; }
    public int SlotDurationMinutes { get; set; } = 30;

    // Contact
    public string? WhatsAppNumber { get; set; }
    public bool WhatsAppEnabled { get; set; } = true;

    // Additional Profile Arrays
    public string? Achievements { get; set; }
    public string? Awards { get; set; }

    // Navigation Properties
    public ICollection<DoctorSpecialization> Specializations { get; set; } = new List<DoctorSpecialization>();
    public ICollection<DoctorQualification> Qualifications { get; set; } = new List<DoctorQualification>();
    public ICollection<DoctorCertification> Certifications { get; set; } = new List<DoctorCertification>();
    public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
    public ICollection<DoctorUnavailability> Unavailabilities { get; set; } = new List<DoctorUnavailability>();
    public ICollection<DoctorPaymentMethod> PaymentMethods { get; set; } = new List<DoctorPaymentMethod>();
}
