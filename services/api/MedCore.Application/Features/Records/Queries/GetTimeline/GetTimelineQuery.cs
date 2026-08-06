using MedCore.Application.Features.Records.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Records.Queries.GetTimeline;

public class GetTimelineQuery : IRequest<List<TimelineEventDto>>
{
    public Guid UserId { get; set; }

    public GetTimelineQuery(Guid userId)
    {
        UserId = userId;
    }
}
