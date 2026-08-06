using System;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;

namespace MedCore.Domain.Entities.Admin;

public class ActivityLog : SoftDeleteEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string ActivityType { get; set; } = null!;
    public string? Description { get; set; }
    public string? Metadata { get; set; } // JSON string
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}
