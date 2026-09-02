using System;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Chat.DTOs;
using MedicHp.Domain.Entities.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedicHp.Application.Features.Chat.Queries.GetConversations;

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
{
    private readonly IGenericRepository<Conversation> _conversationRepository;

    public GetConversationsQueryHandler(IGenericRepository<Conversation> conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<List<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var query = _conversationRepository.GetQueryable()
            .Where(c => c.PatientId == request.UserId || c.DoctorId == request.UserId)
            .Include(c => c.Patient)
            .Include(c => c.Doctor)
                .ThenInclude(d => d.DoctorProfile)
                    .ThenInclude(dp => dp!.Specializations)
                        .ThenInclude(s => s.Specialization)
            .Select(c => new
            {
                Conversation = c,
                OtherUser = c.PatientId == request.UserId ? c.Doctor : c.Patient,
                UnreadCount = c.Messages.Count(m => !m.IsRead && m.SenderId != request.UserId),
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()
            });

        var data = await query.ToListAsync(cancellationToken);

        var result = data.Select(d => new ConversationDto
        {
            Id = d.Conversation.Id,
            OtherParticipantId = d.OtherUser.Id,
            OtherParticipantName = $"{d.OtherUser.FirstName} {d.OtherUser.LastName}".Trim(),
            OtherParticipantPhotoUrl = null,
            OtherParticipantPhoneNumber = d.OtherUser.PhoneNumber,
            OtherParticipantSpecialty = d.OtherUser.DoctorProfile?.Specializations.FirstOrDefault()?.Specialization.Name ?? d.OtherUser.DoctorProfile?.ProfessionalType,
            UnreadCount = d.UnreadCount,
            LastMessage = d.LastMessage != null ? new ChatMessageDto
            {
                Id = d.LastMessage.Id,
                ConversationId = d.LastMessage.ConversationId,
                SenderId = d.LastMessage.SenderId,
                Content = d.LastMessage.Content,
                MessageType = d.LastMessage.MessageType,
                AttachmentId = d.LastMessage.AttachmentId,
                SentAt = d.LastMessage.SentAt,
                IsRead = d.LastMessage.IsRead
            } : null
        }).OrderByDescending(c => c.LastMessage?.SentAt ?? DateTime.MinValue).ToList();

        return result;
    }
}
