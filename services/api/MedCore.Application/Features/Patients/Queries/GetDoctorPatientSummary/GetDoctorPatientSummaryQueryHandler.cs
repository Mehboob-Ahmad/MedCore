using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Shared.Exceptions;
using MedCore.Application.Common;
using MedCore.Application.Features.Patients.DTOs;
using MedCore.Application.Features.Records.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Patients.Queries.GetDoctorPatientSummary;

public class GetDoctorPatientSummaryQueryHandler : IRequestHandler<GetDoctorPatientSummaryQuery, DoctorPatientSummaryDto>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public GetDoctorPatientSummaryQueryHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<Consultation> consultationRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _consultationRepository = consultationRepository;
    }

    public async Task<DoctorPatientSummaryDto> Handle(GetDoctorPatientSummaryQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.PatientId,
            include: q => q.Include(p => p.User)
                           .Include(p => p.Allergies)
                           .Include(p => p.ChronicConditions)
                           .Include(p => p.Medications),
            cancellationToken: cancellationToken);

        if (patient == null)
        {
            throw new NotFoundException(nameof(PatientProfile), request.PatientId);
        }

        var consultations = await _consultationRepository.GetAsync(
            c => c.PatientId == request.PatientId && c.DoctorId == request.DoctorId,
            cancellationToken: cancellationToken);

        return new DoctorPatientSummaryDto
        {
            PatientId = patient.UserId,
            FirstName = patient.User?.FirstName ?? "",
            LastName = patient.User?.LastName ?? "",
            Email = patient.User?.Email ?? "",
            PhoneNumber = patient.User?.PhoneNumber ?? "",
            DateOfBirth = patient.DateOfBirth.HasValue ? patient.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
            Gender = patient.Gender ?? string.Empty,
            BloodGroup = patient.BloodType ?? string.Empty,
            
            Allergies = patient.Allergies.Select(a => new AllergyDto
            {
                Id = a.Id,
                AllergyName = a.AllergyName,
                Severity = a.Severity
            }).ToList(),
            
            ChronicConditions = patient.ChronicConditions.Select(c => new ChronicConditionDto
            {
                Id = c.Id,
                ConditionName = c.ConditionName,
                DiagnosedDate = c.DiagnosedDate.HasValue ? c.DiagnosedDate.Value.ToDateTime(TimeOnly.MinValue) : null
            }).ToList(),
            
            Medications = patient.Medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                MedicationName = m.MedicationName,
                Dosage = m.Dosage,
                Frequency = m.Frequency
            }).ToList(),

            TotalConsultations = consultations.Count(),
            LastConsultationDate = consultations.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.CreatedAt
        };
    }
}
