namespace MedCore.Application.Common;

public interface ICurrentUserProvider
{
    Guid? UserId { get; }
}
