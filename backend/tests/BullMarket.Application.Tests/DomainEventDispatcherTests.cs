using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Domain.Orders;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BullMarket.Application.Tests;

public class DomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_ResolvesAndExecutesMatchingHandlers()
    {
        var services = new ServiceCollection();
        var handler = Substitute.For<IDomainEventHandler<OrderCreatedDomainEvent>>();
        services.AddSingleton(handler);

        var provider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(provider, NullLogger<DomainEventDispatcher>.Instance);

        var domainEvent = new OrderCreatedDomainEvent(
            Guid.NewGuid(), "BM-001", Guid.NewGuid(), 500m, DateTimeOffset.UtcNow);

        await dispatcher.DispatchAsync([domainEvent], CancellationToken.None);

        await handler.Received(1).HandleAsync(domainEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Order_WhenCreatedAndCancelled_CollectsDomainEvents()
    {
        var address = new BullMarket.Domain.Customers.Address("Test", "L1", null, "City", "State", "12345", "US");
        var order = Order.Create(
            "BM-001",
            Guid.NewGuid(),
            [new OrderLine(Guid.NewGuid(), "SKU", "Prod", 100m, 1)],
            address,
            address,
            0m,
            18m,
            "INR",
            DateTimeOffset.UtcNow);

        order.DomainEvents.Should().HaveCount(1);
        order.DomainEvents.First().Should().BeOfType<OrderCreatedDomainEvent>();

        order.Cancel(DateTimeOffset.UtcNow.AddMinutes(5));

        order.DomainEvents.Should().HaveCount(2);
        order.DomainEvents.Last().Should().BeOfType<OrderCancelledDomainEvent>();

        order.ClearDomainEvents();
        order.DomainEvents.Should().BeEmpty();
    }
}
