using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Doctors.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Doctors.Queries.GetDoctorProfile;

public class GetDoctorProfileQueryHandler : IRequestHandler<GetDoctorProfileQuery, DoctorProfileDto>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;

    public GetDoctorProfileQueryHandler(IGenericRepository<DoctorProfile> doctorProfileRepository)
    {
        _doctorProfileRepository = doctorProfileRepository;
    }

    public async Task<DoctorProfileDto> Handle(GetDoctorProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            include: q => q.Include(p => p.User)
                           .Include(p => p.Specializations).ThenInclude(s => s.Specialization)
                           .Include(p => p.Qualifications)
                           .Include(p => p.Certifications)
                           .Include(p => p.Availabilities)
                           .Include(p => p.PaymentMethods),
            cancellationToken: cancellationToken);

        if (profile == null)
        {
            throw new NotFoundException(nameof(DoctorProfile), request.UserId);
        }

        return new DoctorProfileDto
        {
            Id = profile.Id,
            FirstName = profile.User?.FirstName ?? string.Empty,
            LastName = profile.User?.LastName ?? string.Empty,
            Email = profile.User?.Email ?? string.Empty,
            PhoneNumber = profile.User?.PhoneNumber ?? string.Empty,
            Bio = profile.Bio ?? string.Empty,
            ConsultationFee = profile.ConsultationFee,
            RegistrationNumber = profile.RegistrationNumber,
            ProfessionalType = profile.ProfessionalType,
            RegulatoryBody = profile.RegulatoryBody ?? string.Empty,
            VerificationStatus = profile.VerificationStatus,
            WhatsAppNumber = profile.WhatsAppNumber ?? string.Empty,
            WhatsAppEnabled = profile.WhatsAppEnabled,
            ExperienceYears = profile.YearsOfExperience,
            Specializations = profile.Specializations.Select(s => s.Specialization?.Name ?? string.Empty).ToList(),
            Qualifications = profile.Qualifications.Select(q => new DoctorQualificationDto 
            { 
                Degree = q.Degree, 
                Institution = q.Institution, 
                CompletionYear = q.CompletionYear 
            }).ToList(),
            Certifications = profile.Certifications.Select(c => new DoctorCertificationDto 
            { 
                Name = c.Name, 
                IssuingOrganization = c.IssuingOrganization, 
                Year = c.Year 
            }).ToList(),
            
            Availabilities = profile.Availabilities.Select(a => new DoctorAvailabilityDto
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime.ToString(@"hh\:mm"),
                EndTime = a.EndTime.ToString(@"hh\:mm")
            }).ToList(),
            
            PaymentMethods = profile.PaymentMethods.Where(pm => pm.IsActive).Select(pm => new DoctorPaymentMethodDto
            {
                Id = pm.Id,
                PaymentMethodType = pm.PaymentMethodType,
                PaymentProvider = pm.PaymentProvider,
                AccountTitle = pm.AccountTitle,
                // Keep full number for copy button, but add masked version
                AccountNumber = pm.AccountNumber,
                MaskedAccountNumber = !string.IsNullOrEmpty(pm.AccountNumber) && pm.AccountNumber.Length >= 4
                    ? new string('*', pm.AccountNumber.Length - 4) + pm.AccountNumber.Substring(pm.AccountNumber.Length - 4)
                    : pm.AccountNumber,
                IBAN = pm.IBAN,
                MaskedIBAN = !string.IsNullOrEmpty(pm.IBAN) && pm.IBAN.Length >= 4
                    ? new string('*', pm.IBAN.Length - 4) + pm.IBAN.Substring(pm.IBAN.Length - 4)
                    : pm.IBAN,
                IsActive = pm.IsActive
            }).ToList()
        };
    }
}
