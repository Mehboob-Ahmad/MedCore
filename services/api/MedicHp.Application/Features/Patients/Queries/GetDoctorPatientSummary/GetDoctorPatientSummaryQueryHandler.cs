using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Shared.Exceptions;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Patients.DTOs;
using MedicHp.Application.Features.Records.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Patients.Queries.GetDoctorPatientSummary;

public class GetDoctorPatientSummaryQueryHandler : IRequestHandler<GetDoctorPatientSummaryQuery, DoctorPatientSummaryDto>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<Consultation> _consultationRepository;
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public GetDoctorPatientSummaryQueryHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<Consultation> consultationRepository,
        IGenericRepository<Appointment> appointmentRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _consultationRepository = consultationRepository;
        _appointmentRepository = appointmentRepository;
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
            
        var appointments = await _appointmentRepository.GetAsync(
            a => a.PatientId == request.PatientId && a.DoctorId == request.DoctorId,
            cancellationToken: cancellationToken);

        if (!consultations.Any() && !appointments.Any())
        {
            throw new UnauthorizedAccessException("You are not authorized to view this patient's profile. You must have an appointment or consultation with this patient.");
        }

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
