using MedCore.Application.Features.Chat.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace MedCore.Application.Features.Chat.Queries.GetConversations;

public class GetConversationsQuery : IRequest<List<ConversationDto>>
{
    public Guid UserId { get; set; }

    public GetConversationsQuery(Guid userId)
    {
        UserId = userId;
    }
}
