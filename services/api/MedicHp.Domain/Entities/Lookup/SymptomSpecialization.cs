using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Lookup;

public class SymptomSpecialization : SoftDeleteEntity
{
    public Guid SymptomId { get; set; }
    public Symptom Symptom { get; set; } = null!;
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
    public int RelevanceScore { get; set; } = 1;
}
