using System;
using System.Collections.Generic;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;

namespace MedCore.Domain.Entities.Messaging;

public class Conversation : SoftDeleteEntity
{
    public Guid PatientId { get; set; }
    public User Patient { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;
    public DateTime? LastMessageAt { get; set; }
    
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
