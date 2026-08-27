using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Patients.DTOs;

public class PatientDashboardDto
{
    public PatientSummaryDto PatientSummary { get; set; } = new();
    public List<UpcomingAppointmentDto> UpcomingAppointments { get; set; } = new();
    public QuickStatsDto QuickStats { get; set; } = new();
}

public class PatientSummaryDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public int ProfileCompletionPct { get; set; }
}

public class UpcomingAppointmentDto
{
    public Guid AppointmentId { get; set; }
    public string DoctorName { get; set; }
    public string Specialty { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Type { get; set; } // Online or InPerson
    public string Status { get; set; }
}

public class QuickStatsDto
{
    public DateTime? LastConsultationDate { get; set; }
    public int UnreadMessagesCount { get; set; }
    public int ActivePrescriptionsCount { get; set; }
}
