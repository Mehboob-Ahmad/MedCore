using System;
using MedCore.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedCore.Application.Features.Doctors.Queries.GetDoctorProfile;

public record GetDoctorProfileQuery(Guid UserId) : IRequest<DoctorProfileDto>;
