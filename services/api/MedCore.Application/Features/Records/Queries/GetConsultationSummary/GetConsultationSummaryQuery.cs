using MedCore.Application.Features.Records.DTOs;
using MediatR;
using System;

namespace MedCore.Application.Features.Records.Queries.GetConsultationSummary;

public class GetConsultationSummaryQuery : IRequest<ConsultationSummaryDto>
{
    public Guid UserId { get; set; }
    public Guid ConsultationId { get; set; }

    public GetConsultationSummaryQuery(Guid userId, Guid consultationId)
    {
        UserId = userId;
        ConsultationId = consultationId;
    }
}
