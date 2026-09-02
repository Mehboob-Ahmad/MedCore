using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Chat.DTOs;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Messaging;
using MedicHp.Shared.Exceptions;

namespace MedicHp.Application.Features.Chat.Commands.CreateOrGetConversation;

public class CreateOrGetConversationCommandHandler : IRequestHandler<CreateOrGetConversationCommand, ConversationDto>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrGetConversationCommandHandler(
        IGenericRepository<Conversation> conversationRepository,
        IGenericRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConversationDto> Handle(CreateOrGetConversationCommand request, CancellationToken cancellationToken)
    {
        var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (targetUser == null)
            throw new NotFoundException(nameof(User), request.TargetUserId);

        var currentUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (currentUser == null)
            throw new NotFoundException(nameof(User), request.UserId);

        // Determine who is Patient and who is Doctor
        Guid patientId, doctorId;
        if (currentUser.PatientProfile != null && targetUser.DoctorProfile != null)
        {
            patientId = currentUser.Id;
            doctorId = targetUser.Id;
        }
        else if (currentUser.DoctorProfile != null && targetUser.PatientProfile != null)
        {
            patientId = targetUser.Id;
            doctorId = currentUser.Id;
        }
        else
        {
            // fallback if profiles aren't included but roles might be implied
            patientId = currentUser.Id;
            doctorId = targetUser.Id;
        }

        // Check if conversation already exists
        var existingConversation = await _conversationRepository.FirstOrDefaultAsync(
            c => (c.PatientId == patientId && c.DoctorId == doctorId) || (c.PatientId == doctorId && c.DoctorId == patientId),
            null,
            cancellationToken);

        if (existingConversation != null)
        {
            return new ConversationDto
            {
                Id = existingConversation.Id,
                OtherParticipantId = targetUser.Id,
                OtherParticipantName = $"{targetUser.FirstName} {targetUser.LastName}".Trim(),
                OtherParticipantPhotoUrl = null,
                UnreadCount = 0
            };
        }

        var newConversation = new Conversation
        {
            PatientId = patientId,
            DoctorId = doctorId
        };

        await _conversationRepository.AddAsync(newConversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConversationDto
        {
            Id = newConversation.Id,
            OtherParticipantId = targetUser.Id,
            OtherParticipantName = $"{targetUser.FirstName} {targetUser.LastName}".Trim(),
            OtherParticipantPhotoUrl = null,
            UnreadCount = 0
        };
    }
}
