using System;
using System.Collections.Generic;
using MedCore.Application.Features.Appointments.DTOs;
using MediatR;

namespace MedCore.Application.Features.Appointments.Queries.GetDoctorAppointments;

public class GetDoctorAppointmentsQuery : IRequest<List<AppointmentDto>>
{
    public Guid DoctorId { get; set; }
    public string? Filter { get; set; } // today, upcoming, completed, cancelled, pending, missed, rescheduled
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? PatientId { get; set; }
    public string? SearchTerm { get; set; }
    
    public GetDoctorAppointmentsQuery(Guid doctorId, string? filter)
    {
        DoctorId = doctorId;
        Filter = filter;
    }
}
