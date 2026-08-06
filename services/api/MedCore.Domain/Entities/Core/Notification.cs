using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Core;

public class Notification : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Channel { get; set; } = "InApp";
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime SentAt { get; set; }
}
