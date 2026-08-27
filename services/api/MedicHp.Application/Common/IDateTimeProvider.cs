namespace MedicHp.Application.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
