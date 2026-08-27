using MediatR;
using MedicHp.Application.Features.Admin.DTOs;

namespace MedicHp.Application.Features.Admin.Queries.GetSystemStats;

public class GetSystemStatsQuery : IRequest<SystemStatsDto>
{
}
