using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Core;

public class UserRole : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
}
