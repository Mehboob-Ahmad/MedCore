using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Messaging;
using MedicHp.Domain.Entities.Core;
using MediatR;
using MedicHp.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;
    private readonly IGenericRepository<ChatMessage> _messageRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(
        IGenericRepository<Conversation> conversationRepository,
        IGenericRepository<ChatMessage> messageRepository,
        IGenericRepository<User> userRepository,
        IPushNotificationService pushNotificationService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _pushNotificationService = pushNotificationService;
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
            MessageType = request.MessageType,
            AttachmentId = request.AttachmentId,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        
        conversation.UpdatedAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var recipientId = conversation.PatientId == request.UserId ? conversation.DoctorId : conversation.PatientId;
        var recipient = await _userRepository.GetByIdAsync(recipientId, cancellationToken);

        if (recipient != null && !string.IsNullOrEmpty(recipient.PushToken))
        {
            await _pushNotificationService.SendPushNotificationAsync(
                recipient.PushToken,
                "New Message",
                message.Content,
                new { url = $"/chat/{conversation.Id}" }
            );
        }

        return message.Id;
    }
}
