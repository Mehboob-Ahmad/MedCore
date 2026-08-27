using MedicHp.Application.Features.Patients.DTOs;
using MediatR;
using System;

namespace MedicHp.Application.Features.Patients.Queries.GetPatientDashboard;

public class GetPatientDashboardQuery : IRequest<PatientDashboardDto>
{
    public Guid UserId { get; set; }

    public GetPatientDashboardQuery(Guid userId)
    {
        UserId = userId;
    }
}
