using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Admin;

public class SystemSetting : SoftDeleteEntity
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string DataType { get; set; } = "String";
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
}
