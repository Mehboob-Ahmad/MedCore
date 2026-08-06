using System;
using System.Collections.Generic;
using MedCore.Application.Features.DoctorSearch.DTOs;
using MediatR;

namespace MedCore.Application.Features.DoctorSearch.Queries.GetRelatedDoctors;

public class GetRelatedDoctorsQuery : IRequest<List<DoctorSearchResultDto>>
{
    public Guid DoctorId { get; set; }
    public int Limit { get; set; } = 5;
}
