using System;
using MedicHp.Application.Features.Patients.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Patients.Queries.GetDoctorPatientSummary;

public record GetDoctorPatientSummaryQuery(Guid DoctorId, Guid PatientId) : IRequest<DoctorPatientSummaryDto>;
