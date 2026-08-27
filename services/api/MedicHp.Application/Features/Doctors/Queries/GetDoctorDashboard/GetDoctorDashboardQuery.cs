using System;
using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;

namespace MedicHp.Application.Features.Doctors.Queries.GetDoctorDashboard;

public record GetDoctorDashboardQuery(Guid UserId) : IRequest<DoctorDashboardDto>;
