using BullMarket.Application.Abstractions;
using BullMarket.Domain.Orders;
using Microsoft.Extensions.Logging;

namespace BullMarket.Application.Orders;

public sealed class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    : IDomainEventHandler<OrderCreatedDomainEvent>
{
    public Task HandleAsync(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Domain event handled: Order {OrderNumber} ({OrderId}) created for customer {CustomerId} with amount {TotalAmount}",
            domainEvent.OrderNumber,
            domainEvent.OrderId,
            domainEvent.CustomerId,
            domainEvent.TotalAmount);

        return Task.CompletedTask;
    }
}
