using System;
using MediatR;

namespace MedicHp.Application.Features.Chat.Commands.MarkConversationRead;

public class MarkConversationReadCommand : IRequest<bool>
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
}
