using System;
using System.Collections.Generic;
using MediatR;

namespace MedicHp.Application.Features.Productivity.Queries.GetClinicalReminders;

public class GetClinicalRemindersQuery : IRequest<List<ClinicalReminderDto>>
{
}

public class ClinicalReminderDto
{
    public string Type { get; set; } = null!; // "DraftConsultation", "UpcomingFollowUp", "PendingAppointment"
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ActionUrl { get; set; } = null!;
    public Guid ReferenceId { get; set; }
    public DateTime? Date { get; set; }
}
