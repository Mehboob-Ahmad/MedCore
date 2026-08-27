using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Lookup;

public class DiseaseSpecialization : SoftDeleteEntity
{
    public Guid DiseaseId { get; set; }
    public Disease Disease { get; set; } = null!;
    
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
    
    public int RelevanceScore { get; set; } = 1;
}
