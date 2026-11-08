namespace BullMarket.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
