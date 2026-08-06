using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Productivity.Queries.GetDoctorAnalytics;

public class GetDoctorAnalyticsQueryHandler : IRequestHandler<GetDoctorAnalyticsQuery, DoctorAnalyticsDto>
{
    private readonly IGenericRepository<Appointment> _appointmentRepo;
    private readonly IGenericRepository<Consultation> _consultationRepo;
    private readonly IGenericRepository<Prescription> _prescriptionRepo;
    private readonly ICurrentUserService _currentUserService;

    public GetDoctorAnalyticsQueryHandler(
        IGenericRepository<Appointment> appointmentRepo,
        IGenericRepository<Consultation> consultationRepo,
        IGenericRepository<Prescription> prescriptionRepo,
        ICurrentUserService currentUserService)
    {
        _appointmentRepo = appointmentRepo;
        _consultationRepo = consultationRepo;
        _prescriptionRepo = prescriptionRepo;
        _currentUserService = currentUserService;
    }

    public async Task<DoctorAnalyticsDto> Handle(GetDoctorAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;
        var today = DateTime.UtcNow.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // Patients seen this week (Completed appointments or consultations)
        var weeklyAppointments = await _appointmentRepo.GetQueryable().AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.ScheduledAt >= startOfWeek && (a.Status == "Completed" || a.Status == "InProgress"))
            .Select(a => new { a.ScheduledAt, a.PatientId })
            .ToListAsync(cancellationToken);

        var patientsToday = weeklyAppointments.Where(a => a.ScheduledAt.Date == today).Select(a => a.PatientId).Distinct().Count();
        var patientsThisWeek = weeklyAppointments.Select(a => a.PatientId).Distinct().Count();

        var patientsThisMonth = await _appointmentRepo.GetQueryable().AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.ScheduledAt >= startOfMonth && (a.Status == "Completed" || a.Status == "InProgress"))
            .Select(a => a.PatientId)
            .Distinct()
            .CountAsync(cancellationToken);

        var consultationsCompleted = await _consultationRepo.GetQueryable().AsNoTracking()
            .CountAsync(c => c.DoctorId == doctorId && c.IsFinalized, cancellationToken);

        var prescriptionsIssued = await _prescriptionRepo.GetQueryable().AsNoTracking()
            .CountAsync(p => p.Consultation.DoctorId == doctorId && p.Consultation.IsFinalized, cancellationToken);

        var pendingFollowUps = await _consultationRepo.GetQueryable().AsNoTracking()
            .CountAsync(c => c.DoctorId == doctorId && c.IsFinalized && c.FollowUpDate != null && c.FollowUpDate >= today, cancellationToken);

        var upcomingAppointments = await _appointmentRepo.GetQueryable().AsNoTracking()
            .CountAsync(a => a.DoctorId == doctorId && a.ScheduledAt >= today && a.Status == "Confirmed", cancellationToken);

        return new DoctorAnalyticsDto
        {
            PatientsToday = patientsToday,
            PatientsThisWeek = patientsThisWeek,
            PatientsThisMonth = patientsThisMonth,
            ConsultationsCompleted = consultationsCompleted,
            PrescriptionsIssued = prescriptionsIssued,
            PendingFollowUps = pendingFollowUps,
            UpcomingAppointments = upcomingAppointments
        };
    }
}
