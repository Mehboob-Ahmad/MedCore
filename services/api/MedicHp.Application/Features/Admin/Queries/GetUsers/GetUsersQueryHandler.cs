using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Application.Features.Admin.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IGenericRepository<User> _userRepository;

    public GetUsersQueryHandler(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userRepository.GetQueryable()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking();

        // 1. Search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(u => u.Email.ToLower().Contains(term) || 
                                     u.FirstName.ToLower().Contains(term) || 
                                     u.LastName.ToLower().Contains(term));
        }

        // 2. Filter by Role
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name.ToLower() == request.Role.ToLower()));
        }

        // 3. Filter by Status
        if (request.AccountStatus.HasValue)
        {
            query = query.Where(u => u.AccountStatus == request.AccountStatus.Value);
        }

        // 4. Sorting
        var isDesc = request.SortDirection?.ToLower() == "desc";
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "lastloginat":
                    query = isDesc ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt);
                    break;
                case "createdat":
                default:
                    query = isDesc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt);
                    break;
            }
        }
        else
        {
            // Default sort
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = $"{u.FirstName} {u.LastName}".Trim(),
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber ?? string.Empty,
            AccountStatus = u.AccountStatus,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt,
            Role = u.UserRoles.FirstOrDefault()?.Role.Name ?? "Unknown"
        }).ToList();

        return new PagedResult<UserDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
