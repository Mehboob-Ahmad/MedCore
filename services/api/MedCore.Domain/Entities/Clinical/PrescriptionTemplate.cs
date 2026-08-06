using System;
using System.Collections.Generic;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class PrescriptionTemplate : SoftDeleteEntity
{
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; } = null!;
    
    public string TemplateName { get; set; } = null!;
    public string? Notes { get; set; }
    
    public ICollection<PrescriptionTemplateItem> Items { get; set; } = new List<PrescriptionTemplateItem>();
}
