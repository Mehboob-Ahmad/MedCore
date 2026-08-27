using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MedicHp.Application.Features.Auth.Commands.UpdatePushToken;

public class UpdatePushTokenCommand : IRequest<bool>
{
    [Required]
    public string PushToken { get; set; } = null!;
}
