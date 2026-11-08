using BullMarket.Domain.Common;

namespace BullMarket.Domain.Orders;

public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    decimal TotalAmount,
    DateTimeOffset OccurredOn) : IDomainEvent;

public sealed record OrderCancelledDomainEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTimeOffset OccurredOn) : IDomainEvent;
