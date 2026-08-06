namespace MedCore.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
