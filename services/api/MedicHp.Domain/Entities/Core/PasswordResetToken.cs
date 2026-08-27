using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Core;

public class PasswordResetToken : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
