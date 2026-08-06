using MedCore.Application.Common;
using MedCore.Domain.Entities.Messaging;
using MediatR;
using MedCore.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedCore.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;
    private readonly IGenericRepository<ChatMessage> _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(
        IGenericRepository<Conversation> conversationRepository,
        IGenericRepository<ChatMessage> messageRepository,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null || (conversation.PatientId != request.UserId && conversation.DoctorId != request.UserId))
            throw new NotFoundException(nameof(Conversation), request.ConversationId);

        var message = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId = request.UserId,
            Content = request.Content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        
        conversation.UpdatedAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: SignalR Push notification

        return message.Id;
    }
}
