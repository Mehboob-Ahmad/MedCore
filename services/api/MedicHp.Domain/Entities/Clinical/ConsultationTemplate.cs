using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class ConsultationTemplate : SoftDeleteEntity
{
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; } = null!;
    
    public string TemplateName { get; set; } = null!;
    
    public string? Diagnosis { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? FollowUpInstructions { get; set; }
}
