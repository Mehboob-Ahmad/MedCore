using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Clinical;

public class ConsultationAddendum : SoftDeleteEntity
{
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public Guid AddedByDoctorId { get; set; }
    public User AddedByDoctor { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Reason { get; set; } = null!;
}
