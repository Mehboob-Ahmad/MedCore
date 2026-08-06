using System;
using System.Collections.Generic;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;

namespace MedCore.Domain.Entities.Clinical;

public class Prescription : SoftDeleteEntity
{
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;
    public Guid PatientId { get; set; }
    public User Patient { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public bool IsSuperseded { get; set; }
    public Guid? SupersededById { get; set; }
    public Prescription? SupersededBy { get; set; }
    public string? Notes { get; set; }
    
    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}
