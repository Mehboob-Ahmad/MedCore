using System;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Clinical;

public class ConsultationVital : SoftDeleteEntity
{
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public int? BloodPressureSystolic { get; set; }
    public int? BloodPressureDiastolic { get; set; }
    public decimal? TemperatureCelsius { get; set; }
    public decimal? WeightKg { get; set; }
    public int? HeartRateBpm { get; set; }
    public string? Notes { get; set; }
}
