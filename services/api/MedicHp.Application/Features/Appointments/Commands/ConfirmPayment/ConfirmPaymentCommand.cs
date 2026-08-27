using System;
using System.Text.Json.Serialization;
using MediatR;

namespace MedicHp.Application.Features.Appointments.Commands.ConfirmPayment;

public class ConfirmPaymentCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DoctorId { get; set; }
    
    public Guid AppointmentId { get; set; }
}
