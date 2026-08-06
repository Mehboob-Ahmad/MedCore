using System;
using MedCore.Application.Features.Appointments.DTOs;
using MediatR;

namespace MedCore.Application.Features.Appointments.Queries.GetAppointmentDetails;

public class GetAppointmentDetailsQuery : IRequest<AppointmentDetailDto?>
{
    public Guid AppointmentId { get; set; }
    public Guid UserId { get; set; }

    public GetAppointmentDetailsQuery(Guid appointmentId, Guid userId)
    {
        AppointmentId = appointmentId;
        UserId = userId;
    }
}
