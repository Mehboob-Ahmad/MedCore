using MediatR;

namespace MedCore.Application.Features.Productivity.Queries.GetDoctorAnalytics;

public class GetDoctorAnalyticsQuery : IRequest<DoctorAnalyticsDto>
{
}

public class DoctorAnalyticsDto
{
    public int PatientsToday { get; set; }
    public int PatientsThisWeek { get; set; }
    public int PatientsThisMonth { get; set; }
    public int ConsultationsCompleted { get; set; }
    public int PrescriptionsIssued { get; set; }
    public int PendingFollowUps { get; set; }
    public int UpcomingAppointments { get; set; }
}
