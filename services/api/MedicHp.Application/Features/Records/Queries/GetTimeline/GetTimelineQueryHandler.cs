using MedicHp.Application.Common;
using MedicHp.Application.Features.Records.DTOs;
using MedicHp.Domain.Entities.Clinical;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace MedicHp.Application.Features.Records.Queries.GetTimeline;

public class GetTimelineQueryHandler : IRequestHandler<GetTimelineQuery, List<TimelineEventDto>>
{
    private readonly IGenericRepository<PatientProfile> _patientProfileRepository;
    private readonly IGenericRepository<Consultation> _consultationRepository;
    private readonly IGenericRepository<Prescription> _prescriptionRepository;

    public GetTimelineQueryHandler(
        IGenericRepository<PatientProfile> patientProfileRepository,
        IGenericRepository<Consultation> consultationRepository,
        IGenericRepository<Prescription> prescriptionRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _consultationRepository = consultationRepository;
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<List<TimelineEventDto>> Handle(GetTimelineQuery request, CancellationToken cancellationToken)
    {
        var profile = await _patientProfileRepository.FirstOrDefaultAsync(p => p.UserId == request.UserId, null, cancellationToken);
        if (profile == null) return new List<TimelineEventDto>();

        var consultations = await _consultationRepository.GetAsync(
            c => c.PatientId == request.UserId,
            include: q => q.Include(c => c.Doctor),
            cancellationToken);

        var prescriptions = await _prescriptionRepository.GetAsync(
            p => p.PatientId == request.UserId,
            include: q => q.Include(p => p.Doctor),
            cancellationToken);

        var timeline = new List<TimelineEventDto>();

        foreach (var c in consultations)
        {
            timeline.Add(new TimelineEventDto
            {
                EventId = Guid.NewGuid(),
                ReferenceId = c.Id,
                EventType = "Consultation",
                EventDate = c.CreatedAt,
                Title = "General Consultation",
                Description = $"Consultation with Dr. {c.Doctor?.FirstName} {c.Doctor?.LastName}",
                DoctorName = $"Dr. {c.Doctor?.LastName}"
            });
        }

        foreach (var p in prescriptions)
        {
            timeline.Add(new TimelineEventDto
            {
                EventId = Guid.NewGuid(),
                ReferenceId = p.Id,
                EventType = "Prescription",
                EventDate = p.IssuedAt,
                Title = "New Prescription Issued",
                Description = "A new prescription was issued.",
                DoctorName = $"Dr. {p.Doctor?.LastName}"
            });
        }

        return timeline.OrderByDescending(t => t.EventDate).ToList();
    }
}
