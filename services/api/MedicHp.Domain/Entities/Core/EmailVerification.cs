using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Core;

public class EmailVerification : SoftDeleteEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int Attempts { get; set; } = 0;
}
