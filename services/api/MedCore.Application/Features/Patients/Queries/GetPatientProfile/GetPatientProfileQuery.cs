using MedCore.Application.Features.Patients.DTOs;
using MediatR;
using System;

namespace MedCore.Application.Features.Patients.Queries.GetPatientProfile;

public class GetPatientProfileQuery : IRequest<PatientProfileDto>
{
    public Guid UserId { get; set; }

    public GetPatientProfileQuery(Guid userId)
    {
        UserId = userId;
    }
}
