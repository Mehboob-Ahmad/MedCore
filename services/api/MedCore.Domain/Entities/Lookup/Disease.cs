using System.Collections.Generic;
using MedCore.Domain.Common;

namespace MedCore.Domain.Entities.Lookup;

public class Disease : SoftDeleteEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<DiseaseSpecialization> DiseaseSpecializations { get; set; } = new List<DiseaseSpecialization>();
}
