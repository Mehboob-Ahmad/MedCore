using System;
using System.Text.Json.Serialization;
using MediatR;

namespace MedCore.Application.Features.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    
    public Guid AppointmentId { get; set; }
    public DateTime NewScheduledDate { get; set; }
    public string NewStartTime { get; set; } = null!; // "HH:mm"
    public string? Reason { get; set; }
}
