namespace MedCore.Application.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
