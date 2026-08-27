using System;
using MedicHp.Application.Features.DoctorSearch.DTOs;
using MediatR;

namespace MedicHp.Application.Features.DoctorSearch.Queries.GetPublicDoctorProfile;

public class GetPublicDoctorProfileQuery : IRequest<DoctorPublicProfileDto>
{
    public Guid DoctorId { get; set; }
}
