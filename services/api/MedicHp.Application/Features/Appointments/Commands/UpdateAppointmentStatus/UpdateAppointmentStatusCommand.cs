using System;
using System.Text.Json.Serialization;
using MediatR;

namespace MedicHp.Application.Features.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DoctorId { get; set; }
    
    public Guid AppointmentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime? SuggestedNewTime { get; set; }
    public string? DoctorNotes { get; set; }
}
