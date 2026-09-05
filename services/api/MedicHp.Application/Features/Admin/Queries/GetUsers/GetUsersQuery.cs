using System;
using System.Collections.Generic;
using MediatR;

namespace MedicHp.Application.Features.Admin.Queries.GetUsers;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class GetUsersQuery : IRequest<List<UserDto>>
{
}
