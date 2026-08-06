using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Doctors.DTOs;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Doctors.Queries.GetDoctorDashboard;

public class GetDoctorDashboardQueryHandler : IRequestHandler<GetDoctorDashboardQuery, DoctorDashboardDto>
{
    private readonly IGenericRepository<DoctorProfile> _doctorProfileRepository;
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<Consultation> _consultationRepository;

    public GetDoctorDashboardQueryHandler(
        IGenericRepository<DoctorProfile> doctorProfileRepository,
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<Consultation> consultationRepository)
    {
        _doctorProfileRepository = doctorProfileRepository;
        _appointmentRepository = appointmentRepository;
        _consultationRepository = consultationRepository;
    }

    public async Task<DoctorDashboardDto> Handle(GetDoctorDashboardQuery request, CancellationToken cancellationToken)
    {
        var profile = await _doctorProfileRepository.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            cancellationToken: cancellationToken);

        if (profile == null) return new DoctorDashboardDto(); // Or throw exception

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var todayAppointments = await _appointmentRepository.GetAsync(
            a => a.DoctorId == request.UserId && a.ScheduledAt >= today && a.ScheduledAt < tomorrow,
            include: q => q.Include(a => a.Patient),
            cancellationToken: cancellationToken);

        var recentConsultations = await _consultationRepository.GetAsync(
            c => c.DoctorId == request.UserId,
            include: q => q.Include(c => c.Patient),
            cancellationToken: cancellationToken);
            
        recentConsultations = recentConsultations.OrderByDescending(c => c.CreatedAt).Take(5).ToList();

        return new DoctorDashboardDto
        {
            TotalPatients = 0, // Would need a distinct count from consultations/appointments in real scenario
            TodayAppointmentsCount = todayAppointments.Count,
            PendingReports = 0,
            RevenueThisMonth = 0m,
            
            TodayAppointments = todayAppointments.OrderBy(a => a.ScheduledAt).Select(a => new DoctorDashboardAppointmentDto
            {
                Id = a.Id,
                PatientName = $"{a.Patient?.FirstName} {a.Patient?.LastName}",
                Time = a.ScheduledAt.ToString("HH:mm"),
                Status = a.Status
            }).ToList(),

            RecentConsultations = recentConsultations.Select(c => new DoctorDashboardConsultationDto
            {
                Id = c.Id,
                PatientName = $"{c.Patient?.FirstName} {c.Patient?.LastName}",
                Date = c.CreatedAt,
                Diagnosis = c.Diagnosis
            }).ToList()
        };
    }
}
