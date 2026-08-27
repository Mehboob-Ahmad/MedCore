using System;
using System.Collections.Generic;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PrescriptionTemplate : SoftDeleteEntity
{
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; } = null!;
    
    public string TemplateName { get; set; } = null!;
    public string? Notes { get; set; }
    
    public ICollection<PrescriptionTemplateItem> Items { get; set; } = new List<PrescriptionTemplateItem>();
}
