using System;
using System.Collections.Generic;
using MediatR;
using MedicHp.Domain.Enums;
using MedicHp.Application.Common;

namespace MedicHp.Application.Features.Admin.Queries.GetUsers;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public AccountStatus AccountStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public AccountStatus? AccountStatus { get; set; }
    public string? SortBy { get; set; } // "createdAt" or "lastLoginAt"
    public string? SortDirection { get; set; } // "asc" or "desc"
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
