using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Application.Configuration;
using BullMarket.Application.Contracts;
using BullMarket.Application.Orders;
using BullMarket.Domain.Customers;
using BullMarket.Domain.Inventory;
using BullMarket.Domain.Orders;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using DomainCart = BullMarket.Domain.Cart.Cart;

namespace BullMarket.Application.Tests.Orders;

public class OrderServiceTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly ICartRepository _carts = Substitute.For<ICartRepository>();
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly IInventoryRepository _inventory = Substitute.For<IInventoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();
    private readonly CommerceOptions _commerceOptions = new();
    private readonly OrderService _sut;

    private readonly AddressDto _sampleAddress = new("John", "Line1", null, "City", "State", "12345", "US", "123");

    public OrderServiceTests()
    {
        _timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        _unitOfWork.ExecuteInTransactionAsync(Arg.Any<Func<CancellationToken, Task<OrderDto>>>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<Func<CancellationToken, Task<OrderDto>>>()(x.Arg<CancellationToken>()));

        _sut = new OrderService(
            _orders,
            _carts,
            _customers,
            _inventory,
            _unitOfWork,
            _timeProvider,
            new BullMarket.Application.Orders.Strategies.TieredShippingStrategy(_commerceOptions),
            new BullMarket.Application.Orders.Strategies.StandardTaxStrategy(_commerceOptions),
            null,
            NullLogger<OrderService>.Instance);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartEmpty_ThrowsApplicationValidationException()
    {
        var customerId = Guid.NewGuid();
        _customers.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(new Customer("Test", "User", "test@example.com", null, DateTimeOffset.UtcNow));

        var emptyCart = new DomainCart(customerId, DateTimeOffset.UtcNow);
        _carts.GetByCustomerIdAsync(customerId, true, Arg.Any<CancellationToken>())
            .Returns(emptyCart);

        var request = new CreateOrderRequest(customerId, _sampleAddress, _sampleAddress);
        var act = () => _sut.CreateOrderAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("*empty cart*");
    }

    [Fact]
    public async Task CreateOrderAsync_WhenValid_ReducesInventoryAndCreatesOrder()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _customers.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(new Customer("Test", "User", "test@example.com", null, DateTimeOffset.UtcNow));

        var cart = new DomainCart(customerId, DateTimeOffset.UtcNow);
        cart.AddItem(productId, "SKU-1", "Product 1", 100m, "INR", 2, 10, DateTimeOffset.UtcNow);
        _carts.GetByCustomerIdAsync(customerId, true, Arg.Any<CancellationToken>())
            .Returns(cart);

        var inventoryItem = new InventoryItem(productId, 10, DateTimeOffset.UtcNow);
        _inventory.GetByProductIdAsync(productId, true, Arg.Any<CancellationToken>())
            .Returns(inventoryItem);

        var request = new CreateOrderRequest(customerId, _sampleAddress, _sampleAddress);
        var result = await _sut.CreateOrderAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Items.Should().HaveCount(1);
        result.Subtotal.Should().Be(200m);
        inventoryItem.QuantityOnHand.Should().Be(8); // Reduced by 2
        cart.Items.Should().BeEmpty(); // Cart cleared

        _orders.Received(1).Add(Arg.Any<Order>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
