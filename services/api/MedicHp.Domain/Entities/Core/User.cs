using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Clinical;

namespace MedicHp.Domain.Entities.Core;

public class User : SoftDeleteEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NormalizedEmail { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public bool PhoneNumberConfirmed { get; set; }
    public string? Gender { get; set; }
    public string PasswordHash { get; set; } = null!;
    public Guid? ProfilePhotoFileId { get; set; }
    public File? ProfilePhotoFile { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SuspensionReason { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Guid? InvitedByUserId { get; set; }
    public User? InvitedByUser { get; set; }
    public string? InvitationToken { get; set; }
    public DateTime? InvitationAcceptedAt { get; set; }
    public DateTime? TermsAcceptedAt { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? PushToken { get; set; }
    
    // Navigation properties
    public PatientProfile? PatientProfile { get; set; }
    public DoctorProfile? DoctorProfile { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
