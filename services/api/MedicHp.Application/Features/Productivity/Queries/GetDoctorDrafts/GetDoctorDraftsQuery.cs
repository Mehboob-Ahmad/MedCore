using System;
using System.Collections.Generic;
using MediatR;

namespace MedicHp.Application.Features.Productivity.Queries.GetDoctorDrafts;

public class GetDoctorDraftsQuery : IRequest<List<DoctorDraftDto>>
{
}

public class DoctorDraftDto
{
    public Guid ConsultationId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public string? Diagnosis { get; set; }
}
