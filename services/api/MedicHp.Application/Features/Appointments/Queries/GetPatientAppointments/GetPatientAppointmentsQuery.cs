using MedicHp.Application.Features.Appointments.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Appointments.Queries.GetPatientAppointments;

public class GetPatientAppointmentsQuery : IRequest<List<AppointmentDto>>
{
    public Guid UserId { get; set; }
    public string? Filter { get; set; } // upcoming, past, pending, cancelled, rejected
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? DoctorId { get; set; }
    
    public GetPatientAppointmentsQuery(Guid userId, string? filter)
    {
        UserId = userId;
        Filter = filter;
    }
}
