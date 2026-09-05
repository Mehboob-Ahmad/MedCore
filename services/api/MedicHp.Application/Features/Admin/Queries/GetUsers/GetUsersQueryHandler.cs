using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Admin.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IGenericRepository<User> _userRepository;

    public GetUsersQueryHandler(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAsync(
            u => true,
            q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role),
            cancellationToken);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber ?? string.Empty,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            Role = u.UserRoles.FirstOrDefault()?.Role.Name ?? "Unknown"
        }).ToList();
    }
}
