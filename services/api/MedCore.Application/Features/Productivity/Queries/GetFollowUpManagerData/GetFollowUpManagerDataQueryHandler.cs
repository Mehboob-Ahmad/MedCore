using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Application.Features.Productivity.Queries.GetFollowUpManagerData;

public class GetFollowUpManagerDataQueryHandler : IRequestHandler<GetFollowUpManagerDataQuery, FollowUpManagerDataDto>
{
    private readonly IGenericRepository<Consultation> _consultationRepo;
    private readonly IGenericRepository<Appointment> _appointmentRepo;
    private readonly ICurrentUserService _currentUserService;

    public GetFollowUpManagerDataQueryHandler(
        IGenericRepository<Consultation> consultationRepo,
        IGenericRepository<Appointment> appointmentRepo,
        ICurrentUserService currentUserService)
    {
        _consultationRepo = consultationRepo;
        _appointmentRepo = appointmentRepo;
        _currentUserService = currentUserService;
    }

    public async Task<FollowUpManagerDataDto> Handle(GetFollowUpManagerDataQuery request, CancellationToken cancellationToken)
    {
        var doctorId = _currentUserService.UserId!.Value;
        var today = DateTime.UtcNow.Date;

        // Get all consultations with a follow up date
        var consultationsWithFollowUp = await _consultationRepo.GetQueryable().AsNoTracking()
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId && c.IsFinalized && c.FollowUpDate != null)
            .ToListAsync(cancellationToken);

        // Get appointments to check if follow-up was completed
        // (If the patient had an appointment on or after the follow up date)
        var appointments = await _appointmentRepo.GetQueryable().AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.Status == "Completed")
            .Select(a => new { a.PatientId, a.ScheduledAt })
            .ToListAsync(cancellationToken);

        var result = new FollowUpManagerDataDto();

        foreach (var c in consultationsWithFollowUp)
        {
            var followUpDate = c.FollowUpDate!.Value.Date;
            
            // Was it completed? (Patient had an appointment on or after the follow up date)
            bool isCompleted = appointments.Any(a => a.PatientId == c.PatientId && a.ScheduledAt.Date >= followUpDate);

            var dto = new FollowUpDto
            {
                ConsultationId = c.Id,
                PatientId = c.PatientId,
                PatientName = $"{c.Patient.FirstName} {c.Patient.LastName}",
                FollowUpDate = followUpDate,
                Instructions = c.FollowUpInstructions,
                Urgency = c.FollowUpUrgency ?? "Normal"
            };

            if (isCompleted)
            {
                result.Completed.Add(dto);
            }
            else if (followUpDate == today)
            {
                result.Today.Add(dto);
            }
            else if (followUpDate > today)
            {
                result.Upcoming.Add(dto);
            }
            else if (followUpDate < today)
            {
                result.Missed.Add(dto);
            }
        }

        result.Today = result.Today.OrderBy(f => f.FollowUpDate).ToList();
        result.Upcoming = result.Upcoming.OrderBy(f => f.FollowUpDate).ToList();
        result.Missed = result.Missed.OrderByDescending(f => f.FollowUpDate).ToList();
        result.Completed = result.Completed.OrderByDescending(f => f.FollowUpDate).ToList();

        return result;
    }
}
