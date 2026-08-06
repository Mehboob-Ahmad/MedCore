using System.Collections.Generic;
using MedCore.Application.Features.Patients.DTOs;
using MediatR;

namespace MedCore.Application.Features.Patients.Queries.SearchMedCorePatients;

public record SearchMedCorePatientsQuery(string? SearchTerm) : IRequest<List<PatientSearchDto>>;
