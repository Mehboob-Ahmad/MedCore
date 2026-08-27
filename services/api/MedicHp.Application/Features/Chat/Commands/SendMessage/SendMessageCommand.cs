using MediatR;
using System;

namespace MedicHp.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = null!;
}
