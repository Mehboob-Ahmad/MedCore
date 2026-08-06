using MedCore.Application.Features.Chat.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Chat.Queries.GetMessages;

public class GetMessagesQuery : IRequest<List<ChatMessageDto>>
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }

    public GetMessagesQuery(Guid userId, Guid conversationId)
    {
        UserId = userId;
        ConversationId = conversationId;
    }
}
