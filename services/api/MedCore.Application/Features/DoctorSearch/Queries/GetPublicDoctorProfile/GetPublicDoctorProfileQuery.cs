using System;
using MedCore.Application.Features.DoctorSearch.DTOs;
using MediatR;

namespace MedCore.Application.Features.DoctorSearch.Queries.GetPublicDoctorProfile;

public class GetPublicDoctorProfileQuery : IRequest<DoctorPublicProfileDto>
{
    public Guid DoctorId { get; set; }
}
