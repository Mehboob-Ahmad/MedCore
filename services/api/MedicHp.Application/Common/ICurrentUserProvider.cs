namespace MedicHp.Application.Common;

public interface ICurrentUserProvider
{
    Guid? UserId { get; }
}
