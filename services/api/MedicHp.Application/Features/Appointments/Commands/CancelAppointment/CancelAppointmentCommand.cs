using MediatR;
using System;

namespace MedicHp.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Reason { get; set; }
}
