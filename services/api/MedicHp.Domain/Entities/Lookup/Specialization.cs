using System.Collections.Generic;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Clinical;

namespace MedicHp.Domain.Entities.Lookup;

public class Specialization : SoftDeleteEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    
    public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    public ICollection<SymptomSpecialization> SymptomSpecializations { get; set; } = new List<SymptomSpecialization>();
    public ICollection<DiseaseSpecialization> DiseaseSpecializations { get; set; } = new List<DiseaseSpecialization>();
}
