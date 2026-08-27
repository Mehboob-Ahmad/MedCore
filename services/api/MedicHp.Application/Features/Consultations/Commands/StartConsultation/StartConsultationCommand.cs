using System;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Commands.StartConsultation;

public class StartConsultationCommand : IRequest<Guid>
{
    public Guid AppointmentId { get; set; }
    public Guid DoctorId { get; set; }
}
