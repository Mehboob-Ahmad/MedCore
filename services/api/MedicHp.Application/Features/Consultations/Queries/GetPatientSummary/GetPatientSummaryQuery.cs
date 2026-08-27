using System;
using MedicHp.Application.Features.Consultations.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Consultations.Queries.GetPatientSummary;

public class GetPatientSummaryQuery : IRequest<PatientSummaryDto?>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; } // For potential authorization checks
}
