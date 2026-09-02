using System;
using MediatR;
using MedicHp.Application.Features.Chat.DTOs;

namespace MedicHp.Application.Features.Chat.Commands.CreateOrGetConversation;

public class CreateOrGetConversationCommand : IRequest<ConversationDto>
{
    public Guid UserId { get; set; }
    public Guid TargetUserId { get; set; }
}
