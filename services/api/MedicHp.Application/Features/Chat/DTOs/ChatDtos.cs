using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Chat.DTOs;

public class ConversationDto
{
    public Guid Id { get; set; }
    public Guid OtherParticipantId { get; set; }
    public string OtherParticipantName { get; set; }
    public string? OtherParticipantPhotoUrl { get; set; }
    
    public ChatMessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}
