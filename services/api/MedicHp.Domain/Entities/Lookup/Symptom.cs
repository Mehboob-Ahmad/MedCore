using System.Collections.Generic;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Lookup;

public class Symptom : SoftDeleteEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<SymptomSpecialization> SymptomSpecializations { get; set; } = new List<SymptomSpecialization>();
}
