using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Messaging;
using MedicHp.Shared.Exceptions;

namespace MedicHp.Application.Features.Chat.Commands.MarkConversationRead;

public class MarkConversationReadCommandHandler : IRequestHandler<MarkConversationReadCommand, bool>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;
    private readonly IGenericRepository<ChatMessage> _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkConversationReadCommandHandler(
        IGenericRepository<Conversation> conversationRepository,
        IGenericRepository<ChatMessage> messageRepository,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkConversationReadCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null || (conversation.PatientId != request.UserId && conversation.DoctorId != request.UserId))
            throw new NotFoundException(nameof(Conversation), request.ConversationId);

        var unreadMessages = await _messageRepository.GetAsync(
            m => m.ConversationId == request.ConversationId && !m.IsRead && m.SenderId != request.UserId,
            null,
            cancellationToken);

        if (!unreadMessages.Any())
            return true;

        var now = DateTime.UtcNow;
        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
            message.ReadAt = now;
            await _messageRepository.UpdateAsync(message, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
