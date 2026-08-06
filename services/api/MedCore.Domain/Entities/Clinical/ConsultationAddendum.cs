using System;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Core;

namespace MedCore.Domain.Entities.Clinical;

public class ConsultationAddendum : SoftDeleteEntity
{
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public Guid AddedByDoctorId { get; set; }
    public User AddedByDoctor { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Reason { get; set; } = null!;
}
