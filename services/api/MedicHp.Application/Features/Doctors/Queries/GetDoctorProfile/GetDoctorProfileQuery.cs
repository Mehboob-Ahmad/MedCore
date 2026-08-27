using System;
using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Doctors.Queries.GetDoctorProfile;

public record GetDoctorProfileQuery(Guid UserId) : IRequest<DoctorProfileDto>;
