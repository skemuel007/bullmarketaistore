using BullMarket.Application.Abstractions;
using BullMarket.Application.Cart;
using BullMarket.Application.Common;
using BullMarket.Application.Contracts;
using BullMarket.Domain.Customers;
using BullMarket.Domain.Inventory;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using DomainCart = BullMarket.Domain.Cart.Cart;

namespace BullMarket.Application.Tests.Cart;

public class CartServiceTests
{
    private readonly ICartRepository _carts = Substitute.For<ICartRepository>();
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly IInventoryRepository _inventory = Substitute.For<IInventoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();
    private readonly CartService _sut;

    public CartServiceTests()
    {
        _timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);
        _sut = new CartService(
            _carts,
            _products,
            _customers,
            _inventory,
            _unitOfWork,
            _timeProvider,
            NullLogger<CartService>.Instance);
    }

    [Fact]
    public async Task GetCartAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        var customerId = Guid.NewGuid();
        _customers.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var act = () => _sut.GetCartAsync(customerId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetCartAsync_WhenCartDoesNotExist_ReturnsEmptyCartDto()
    {
        var customerId = Guid.NewGuid();
        _customers.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(new Customer("Test", "User", "test@example.com", null, DateTimeOffset.UtcNow));
        _carts.GetByCustomerIdAsync(customerId, false, Arg.Any<CancellationToken>())
            .Returns((DomainCart?)null);

        var result = await _sut.GetCartAsync(customerId, CancellationToken.None);

        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Items.Should().BeEmpty();
        result.Subtotal.Should().Be(0m);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductInactive_ThrowsApplicationValidationException()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _customers.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(new Customer("Test", "User", "test@example.com", null, DateTimeOffset.UtcNow));

        var product = new Product("SKU-1", "Name", "slug", "Brand", Guid.NewGuid(), "Short", "Desc", 100m, "INR", DateTimeOffset.UtcNow);
        product.Deactivate(DateTimeOffset.UtcNow);

        _products.GetByIdAsync(productId, false, Arg.Any<CancellationToken>()).Returns(product);

        var act = () => _sut.AddItemAsync(customerId, new AddCartItemRequest(productId, 2), CancellationToken.None);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("*inactive*");
    }

    [Fact]
    public async Task AddItemAsync_WhenValid_AddsItemAndSavesChanges()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer("Test", "User", "test@example.com", null, DateTimeOffset.UtcNow);
        _customers.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        var product = new Product("SKU-1", "Name", "slug", "Brand", Guid.NewGuid(), "Short", "Desc", 100m, "INR", DateTimeOffset.UtcNow);
        var productId = product.Id;
        _products.GetByIdAsync(productId, false, Arg.Any<CancellationToken>()).Returns(product);

        var inventoryItem = new InventoryItem(productId, 10, DateTimeOffset.UtcNow);
        _inventory.GetByProductIdAsync(productId, false, Arg.Any<CancellationToken>()).Returns(inventoryItem);

        _carts.GetByCustomerIdAsync(customerId, true, Arg.Any<CancellationToken>())
            .Returns((DomainCart?)null);

        var result = await _sut.AddItemAsync(customerId, new AddCartItemRequest(productId, 3), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.First().ProductId.Should().Be(productId);
        result.Items.First().Quantity.Should().Be(3);
        result.Subtotal.Should().Be(300m);

        _carts.Received(1).Add(Arg.Any<DomainCart>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
