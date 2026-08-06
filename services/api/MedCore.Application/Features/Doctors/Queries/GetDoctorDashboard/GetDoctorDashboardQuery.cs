using System;
using MedCore.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedCore.Application.Features.Doctors.Queries.GetDoctorDashboard;

public record GetDoctorDashboardQuery(Guid UserId) : IRequest<DoctorDashboardDto>;
