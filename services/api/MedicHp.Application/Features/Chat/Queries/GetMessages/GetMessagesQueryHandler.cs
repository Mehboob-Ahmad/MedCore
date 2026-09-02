using MedicHp.Application.Common;
using MedicHp.Application.Features.Chat.DTOs;
using MedicHp.Domain.Entities.Messaging;
using MediatR;
using MedicHp.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Chat.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<ChatMessageDto>>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;
    private readonly IGenericRepository<ChatMessage> _messageRepository;

    public GetMessagesQueryHandler(
        IGenericRepository<Conversation> conversationRepository,
        IGenericRepository<ChatMessage> messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<List<ChatMessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null || (conversation.PatientId != request.UserId && conversation.DoctorId != request.UserId))
            throw new NotFoundException(nameof(Conversation), request.ConversationId);

        var messages = await _messageRepository.GetAsync(
            m => m.ConversationId == request.ConversationId,
            include: q => q.OrderBy(m => m.SentAt),
            cancellationToken);

        return messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            Content = m.Content,
            MessageType = m.MessageType,
            AttachmentId = m.AttachmentId,
            AttachmentUrl = m.AttachmentId.HasValue ? $"/api/v1/chat/attachments/{m.AttachmentId.Value}" : null,
            SentAt = m.SentAt,
            IsRead = m.IsRead
        }).ToList();
    }
}
