using System;
using MedCore.Application.Features.Patients.DTOs;
using MediatR;

namespace MedCore.Application.Features.Patients.Queries.GetDoctorPatientSummary;

public record GetDoctorPatientSummaryQuery(Guid DoctorId, Guid PatientId) : IRequest<DoctorPatientSummaryDto>;
