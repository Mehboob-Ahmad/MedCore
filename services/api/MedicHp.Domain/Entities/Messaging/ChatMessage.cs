using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Messaging;

public class ChatMessage : SoftDeleteEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;
    public string? Content { get; set; }
    public string MessageType { get; set; } = "TEXT"; // TEXT, IMAGE, VIDEO, VOICE
    public Guid? AttachmentId { get; set; }
    public MedicHp.Domain.Entities.Core.File? Attachment { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
