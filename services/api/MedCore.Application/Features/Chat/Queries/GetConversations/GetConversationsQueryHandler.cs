using MedCore.Application.Common;
using MedCore.Application.Features.Chat.DTOs;
using MedCore.Domain.Entities.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Chat.Queries.GetConversations;

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;

    public GetConversationsQueryHandler(IGenericRepository<Conversation> conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<List<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _conversationRepository.GetAsync(
            c => c.PatientId == request.UserId || c.DoctorId == request.UserId,
            include: q => q.Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1)),
            cancellationToken);

        var result = new List<ConversationDto>();
        foreach (var c in conversations)
        {
            var isPatient = c.PatientId == request.UserId;
            var otherParticipantId = isPatient ? c.DoctorId : c.PatientId;

            var lastMessage = c.Messages.FirstOrDefault();
            
            result.Add(new ConversationDto
            {
                Id = c.Id,
                OtherParticipantId = otherParticipantId,
                OtherParticipantName = "Unknown", // Needs to be fetched via User repo
                UnreadCount = 0, // Needs calculation
                LastMessage = lastMessage != null ? new ChatMessageDto
                {
                    Id = lastMessage.Id,
                    ConversationId = lastMessage.ConversationId,
                    SenderId = lastMessage.SenderId,
                    Content = lastMessage.Content,
                    SentAt = lastMessage.SentAt,
                    IsRead = lastMessage.IsRead
                } : null
            });
        }

        return result.OrderByDescending(c => c.LastMessage?.SentAt).ToList();
    }
}
