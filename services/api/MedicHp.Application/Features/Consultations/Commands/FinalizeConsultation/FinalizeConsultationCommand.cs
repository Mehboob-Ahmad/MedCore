using System;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Commands.FinalizeConsultation;

public class FinalizeConsultationCommand : IRequest<Unit>
{
    public Guid ConsultationId { get; set; }
    public Guid DoctorId { get; set; }
}
