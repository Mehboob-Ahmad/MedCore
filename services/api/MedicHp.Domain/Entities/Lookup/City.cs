using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Lookup;

public class City : SoftDeleteEntity
{
    public string Name { get; set; } = null!;
    public string? StateOrProvince { get; set; }
    public string Country { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
