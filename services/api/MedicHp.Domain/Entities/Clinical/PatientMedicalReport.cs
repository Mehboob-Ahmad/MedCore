using System;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientMedicalReport : BaseEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    
    public string ReportName { get; set; } = null!;
    public string ReportType { get; set; } = null!; // e.g. Lab, X-Ray, Prescription
    public DateTime? ReportDate { get; set; }
    public string? Notes { get; set; }
    
    public Guid FileId { get; set; }
    public MedicHp.Domain.Entities.Core.File File { get; set; } = null!;
}
