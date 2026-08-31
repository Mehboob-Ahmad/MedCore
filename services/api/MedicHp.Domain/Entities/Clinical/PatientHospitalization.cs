using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Clinical;

public class PatientHospitalization : BaseEntity
{
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    
    public string Reason { get; set; } = null!;
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? HospitalName { get; set; }
    public string? Notes { get; set; }
}
