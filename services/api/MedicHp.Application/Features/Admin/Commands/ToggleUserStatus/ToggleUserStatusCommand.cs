using System;
using MediatR;
using MedicHp.Domain.Enums;

namespace MedicHp.Application.Features.Admin.Commands.ToggleUserStatus;

public class ToggleUserStatusCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public string? Reason { get; set; }
}
