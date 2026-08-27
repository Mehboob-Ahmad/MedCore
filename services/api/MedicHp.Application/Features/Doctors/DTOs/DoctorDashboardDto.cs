using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Doctors.DTOs;

public class DoctorDashboardDto
{
    public int TotalPatients { get; set; }
    public int TodayAppointmentsCount { get; set; }
    public int PendingReports { get; set; }
    public decimal RevenueThisMonth { get; set; }

    public List<DoctorDashboardAppointmentDto> TodayAppointments { get; set; } = new();
    public List<DoctorDashboardConsultationDto> RecentConsultations { get; set; } = new();
}

public class DoctorDashboardAppointmentDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class DoctorDashboardConsultationDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
}
