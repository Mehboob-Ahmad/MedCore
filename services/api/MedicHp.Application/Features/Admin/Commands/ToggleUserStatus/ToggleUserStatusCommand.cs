using System;
using MediatR;

namespace MedicHp.Application.Features.Admin.Commands.ToggleUserStatus;

public class ToggleUserStatusCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public string? Reason { get; set; }
}
