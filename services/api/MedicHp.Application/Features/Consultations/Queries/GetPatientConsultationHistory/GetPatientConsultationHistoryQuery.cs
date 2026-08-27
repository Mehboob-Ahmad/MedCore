using System;
using System.Collections.Generic;
using MedicHp.Application.Features.Consultations.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Queries.GetPatientConsultationHistory;

public class GetPatientConsultationHistoryQuery : IRequest<List<ConsultationSummaryDto>>
{
    public Guid PatientId { get; set; }
    public Guid UserId { get; set; } // Context user (doctor or patient)
}
