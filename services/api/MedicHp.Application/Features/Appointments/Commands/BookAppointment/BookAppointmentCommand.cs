using MediatR;
using System;

namespace MedicHp.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string StartTime { get; set; } = null!; // "HH:mm"
    public string? BookingNote { get; set; }
}
