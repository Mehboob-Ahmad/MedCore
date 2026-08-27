using System;
using MedicHp.Application.Features.Consultations.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Queries.GetConsultationDetails;

public class GetConsultationDetailsQuery : IRequest<ConsultationDto?>
{
    public Guid ConsultationId { get; set; }
    public Guid UserId { get; set; } // Context user (doctor or patient)
    public bool IsDoctor { get; set; }
}
