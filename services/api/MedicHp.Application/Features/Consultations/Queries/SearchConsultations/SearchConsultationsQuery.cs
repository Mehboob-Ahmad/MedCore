using System;
using System.Collections.Generic;
using MedicHp.Application.Features.Consultations.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Queries.SearchConsultations;

public class SearchConsultationsQuery : IRequest<List<ConsultationSummaryDto>>
{
    public Guid DoctorId { get; set; }
    public string? Query { get; set; } // Patient name, diagnosis, etc.
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
