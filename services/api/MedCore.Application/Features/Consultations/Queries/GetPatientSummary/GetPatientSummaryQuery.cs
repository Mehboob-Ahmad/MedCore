using System;
using MedCore.Application.Features.Consultations.DTOs;
using MediatR;

namespace MedCore.Application.Features.Consultations.Queries.GetPatientSummary;

public class GetPatientSummaryQuery : IRequest<PatientSummaryDto?>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; } // For potential authorization checks
}
