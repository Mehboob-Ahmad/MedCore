using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientSurgeryDocument : BaseEntity
{
    public Guid PatientSurgeryId { get; set; }
    public PatientSurgery PatientSurgery { get; set; } = null!;
    
    public Guid FileId { get; set; }
    public MedicHp.Domain.Entities.Core.File File { get; set; } = null!;
    
    public string? Description { get; set; }
}
