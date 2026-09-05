using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Doctors.DTOs;

public class DoctorProfileDto
{
    public Guid Id { get; set; }
    
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfilePhotoUrl { get; set; } = string.Empty;
    
    // Professional Information
    public string ProfessionalType { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string RegulatoryBody { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Bio { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public List<string> Specializations { get; set; } = new();
    public List<DoctorQualificationDto> Qualifications { get; set; } = new();
    public List<DoctorCertificationDto> Certifications { get; set; } = new();
    public List<string> Languages { get; set; } = new();
    
    // WhatsApp Contact
    public string WhatsAppNumber { get; set; } = string.Empty;
    public bool WhatsAppEnabled { get; set; }

    // Clinic Information
    public string ClinicName { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
    public Guid? CityId { get; set; }
    public string ClinicCity { get; set; } = string.Empty;
    public string ClinicPhoneNumber { get; set; } = string.Empty;
    public string GoogleMapsUrl { get; set; } = string.Empty;

    public List<DoctorAvailabilityDto> Availabilities { get; set; } = new();
    
    public List<DoctorPaymentMethodDto> PaymentMethods { get; set; } = new();
}

public class DoctorQualificationDto
{
    public string Degree { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public int CompletionYear { get; set; }
}

public class DoctorCertificationDto
{
    public string Name { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public int? Year { get; set; }
}

public class DoctorAvailabilityDto
{
    public short DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
