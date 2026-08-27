using MedicHp.Application.Features.Records.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Records.Queries.GetTimeline;

public class GetTimelineQuery : IRequest<List<TimelineEventDto>>
{
    public Guid UserId { get; set; }

    public GetTimelineQuery(Guid userId)
    {
        UserId = userId;
    }
}
