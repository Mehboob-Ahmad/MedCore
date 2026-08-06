using System;
using System.Collections.Generic;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Core;

public class Role : SoftDeleteEntity
{
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string? Description { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
