namespace MedicHp.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
