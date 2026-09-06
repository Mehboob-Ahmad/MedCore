using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Clinical;

public class AiChatMessage : AuditableEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    // "user" or "model"
    public string Role { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
}
