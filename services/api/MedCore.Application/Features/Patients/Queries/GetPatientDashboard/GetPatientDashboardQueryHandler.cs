using MedCore.Application.Common;
using MedCore.Application.Features.Patients.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedCore.Shared.Exceptions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace MedCore.Application.Features.Patients.Queries.GetPatientDashboard;

public class GetPatientDashboardQueryHandler : IRequestHandler<GetPatientDashboardQuery, PatientDashboardDto>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public GetPatientDashboardQueryHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<Appointment> appointmentRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<PatientDashboardDto> Handle(GetPatientDashboardQuery request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            include: q => q.Include(p => p.User).Include(p => p.Medications),
            cancellationToken);

        if (profile == null)
            throw new NotFoundException(nameof(PatientProfile), request.UserId);

        var upcomingAppointments = await _appointmentRepository.GetAsync(
            a => a.PatientId == request.UserId && a.ScheduledAt >= DateTime.UtcNow && a.Status == "Scheduled",
            include: q => q.Include(a => a.Doctor),
            cancellationToken);

        var pastConsultations = await _appointmentRepository.GetAsync(
            a => a.PatientId == request.UserId && a.Status == "Completed",
            include: null,
            cancellationToken);

        var lastConsultation = pastConsultations
            .OrderByDescending(a => a.ScheduledAt)
            .FirstOrDefault();

        return new PatientDashboardDto
        {
            PatientSummary = new PatientSummaryDto
            {
                FirstName = profile.User?.FirstName ?? "",
                LastName = profile.User?.LastName ?? "",
                ProfilePhotoUrl = null,
                ProfileCompletionPct = profile.ProfileCompletionPct
            },
            UpcomingAppointments = upcomingAppointments
                .OrderBy(a => a.ScheduledAt)
                .Take(5)
                .Select(a => new UpcomingAppointmentDto
                {
                    AppointmentId = a.Id,
                    DoctorName = $"Dr. {a.Doctor?.FirstName} {a.Doctor?.LastName}",
                    Specialty = "General", // Mocked
                    ScheduledDate = a.ScheduledAt,
                    Type = "InPerson", // Mocked
                    Status = a.Status
                }).ToList(),
            QuickStats = new QuickStatsDto
            {
                LastConsultationDate = lastConsultation?.ScheduledAt,
                UnreadMessagesCount = 0, // Mocked for now
                ActivePrescriptionsCount = profile.Medications.Count
            }
        };
    }
}
