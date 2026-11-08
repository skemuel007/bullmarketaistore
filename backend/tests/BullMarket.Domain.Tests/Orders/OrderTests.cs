using BullMarket.Domain.Common;
using BullMarket.Domain.Customers;
using BullMarket.Domain.Orders;
using FluentAssertions;

namespace BullMarket.Domain.Tests.Orders;

public class OrderTests
{
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Address _address = new("Jane Doe", "123 Main St", null, "City", "State", "12345", "US");
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    private Order CreateSampleOrder()
    {
        var lines = new[]
        {
            new OrderLine(Guid.NewGuid(), "SKU-1", "Product 1", 100m, 2),
            new OrderLine(Guid.NewGuid(), "SKU-2", "Product 2", 50m, 1)
        };

        return Order.Create(
            "ORD-001",
            _customerId,
            lines,
            _address,
            _address,
            10m,
            25m,
            "INR",
            _now);
    }

    [Fact]
    public void Create_CalculatesTotalsAndSetsInitialStatus()
    {
        var order = CreateSampleOrder();

        order.Id.Should().NotBeEmpty();
        order.OrderNumber.Should().Be("ORD-001");
        order.CustomerId.Should().Be(_customerId);
        order.Items.Should().HaveCount(2);
        order.Subtotal.Should().Be(250m);
        order.ShippingAmount.Should().Be(10m);
        order.TaxAmount.Should().Be(25m);
        order.TotalAmount.Should().Be(285m);
        order.OrderStatus.Should().Be(OrderStatus.Pending);
        order.PaymentStatus.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Lifecycle_TransitionsThroughStatusesCorrectly()
    {
        var order = CreateSampleOrder();

        order.Confirm();
        order.OrderStatus.Should().Be(OrderStatus.Confirmed);

        order.StartProcessing();
        order.OrderStatus.Should().Be(OrderStatus.Processing);

        order.MarkShipped();
        order.OrderStatus.Should().Be(OrderStatus.Shipped);

        order.MarkDelivered();
        order.OrderStatus.Should().Be(OrderStatus.Delivered);

        order.MarkReturned();
        order.OrderStatus.Should().Be(OrderStatus.Returned);
    }

    [Fact]
    public void InvalidTransition_ThrowsDomainException()
    {
        var order = CreateSampleOrder();

        // Cannot jump directly from Pending to Shipped
        var act = () => order.MarkShipped();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_WhenPending_SetsStatusToCancelled()
    {
        var order = CreateSampleOrder();
        var cancelDate = _now.AddDays(1);

        order.Cancel(cancelDate);

        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        order.CancelledAt.Should().Be(cancelDate);
    }

    [Fact]
    public void Cancel_WhenAlreadyDelivered_ThrowsDomainException()
    {
        var order = CreateSampleOrder();
        order.Confirm();
        order.StartProcessing();
        order.MarkShipped();
        order.MarkDelivered();

        var act = () => order.Cancel(_now.AddDays(2));

        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot cancel order*");
    }
}
