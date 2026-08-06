using System;
using System.Collections.Generic;
using MediatR;

namespace MedCore.Application.Features.Productivity.Queries.GetFollowUpManagerData;

public class GetFollowUpManagerDataQuery : IRequest<FollowUpManagerDataDto>
{
}

public class FollowUpManagerDataDto
{
    public List<FollowUpDto> Today { get; set; } = new();
    public List<FollowUpDto> Upcoming { get; set; } = new();
    public List<FollowUpDto> Missed { get; set; } = new();
    public List<FollowUpDto> Completed { get; set; } = new();
}

public class FollowUpDto
{
    public Guid ConsultationId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public DateTime FollowUpDate { get; set; }
    public string? Instructions { get; set; }
    public string Urgency { get; set; } = null!;
}
