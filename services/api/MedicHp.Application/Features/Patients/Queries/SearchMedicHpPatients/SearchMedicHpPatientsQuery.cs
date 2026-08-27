using System.Collections.Generic;
using MedicHp.Application.Features.Patients.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Patients.Queries.SearchMedicHpPatients;

public record SearchMedicHpPatientsQuery(string? SearchTerm) : IRequest<List<PatientSearchDto>>;
