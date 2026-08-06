using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Productivity.Queries.GetClinicalReminders;

public class GetClinicalRemindersQueryHandler : IRequestHandler<GetClinicalRemindersQuery, List<ClinicalReminderDto>>
{
    private readonly IGenericRepository<Appointment> _appointmentRepo;
    private readonly IGenericRepository<Consultation> _consultationRepo;
    private readonly ICurrentUserService _currentUserService;

    public GetClinicalRemindersQueryHandler(
        IGenericRepository<Appointment> appointmentRepo,
        IGenericRepository<Consultation> consultationRepo,
        ICurrentUserService currentUserService)
    {
        _appointmentRepo = appointmentRepo;
        _consultationRepo = consultationRepo;
        _currentUserService = currentUserService;
    }

    public async Task<List<ClinicalReminderDto>> Handle(GetClinicalRemindersQuery request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;
        var today = DateTime.UtcNow.Date;
        var reminders = new List<ClinicalReminderDto>();

        // 1. Draft Consultations
        var drafts = await _consultationRepo.GetQueryable().AsNoTracking()
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId && !c.IsFinalized)
            .ToListAsync(cancellationToken);

        reminders.AddRange(drafts.Select(d => new ClinicalReminderDto
        {
            Type = "DraftConsultation",
            Title = $"Draft: {d.Patient.FirstName} {d.Patient.LastName}",
            Description = "You have an unfinalized consultation.",
            ActionUrl = $"/consultations/{d.Id}",
            ReferenceId = d.Id,
            Date = d.CreatedAt
        }));

        // 2. Upcoming Follow-ups (Next 7 days)
        var nextWeek = today.AddDays(7);
        var followUps = await _consultationRepo.GetQueryable().AsNoTracking()
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId && c.IsFinalized && c.FollowUpDate >= today && c.FollowUpDate <= nextWeek)
            .ToListAsync(cancellationToken);

        reminders.AddRange(followUps.Select(f => new ClinicalReminderDto
        {
            Type = "UpcomingFollowUp",
            Title = $"Follow-up: {f.Patient.FirstName} {f.Patient.LastName}",
            Description = f.FollowUpInstructions ?? "Scheduled follow-up.",
            ActionUrl = $"/patients/{f.PatientId}",
            ReferenceId = f.PatientId,
            Date = f.FollowUpDate
        }));

        // 3. Pending Appointments (Today)
        var appointments = await _appointmentRepo.GetQueryable().AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.ScheduledAt.Date == today && a.Status == "Confirmed")
            .ToListAsync(cancellationToken);

        reminders.AddRange(appointments.Select(a => new ClinicalReminderDto
        {
            Type = "PendingAppointment",
            Title = $"Appointment: {a.Patient.FirstName} {a.Patient.LastName}",
            Description = $"Appointment at {a.ScheduledAt:t}",
            ActionUrl = $"/appointments/{a.Id}",
            ReferenceId = a.Id,
            Date = a.ScheduledAt
        }));

        return reminders.OrderBy(r => r.Date).ToList();
    }
}
