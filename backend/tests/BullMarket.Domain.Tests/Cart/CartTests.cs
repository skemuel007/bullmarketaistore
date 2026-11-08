using BullMarket.Domain.Cart;
using BullMarket.Domain.Common;
using FluentAssertions;

namespace BullMarket.Domain.Tests.Cart;

public class CartTests
{
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Constructor_WithValidArguments_InitializesCart()
    {
        var cart = new Domain.Cart.Cart(_customerId, _now);

        cart.Id.Should().NotBeEmpty();
        cart.CustomerId.Should().Be(_customerId);
        cart.CreatedAt.Should().Be(_now);
        cart.UpdatedAt.Should().Be(_now);
        cart.Items.Should().BeEmpty();
        cart.Subtotal.Should().Be(0m);
    }

    [Fact]
    public void Constructor_WithEmptyCustomerId_ThrowsDomainException()
    {
        var act = () => new Domain.Cart.Cart(Guid.Empty, _now);

        act.Should().Throw<DomainException>()
            .WithMessage("*Customer ID*");
    }

    [Fact]
    public void AddItem_WhenNewItem_AddsToCartAndCalculatesSubtotal()
    {
        var cart = new BullMarket.Domain.Cart.Cart(_customerId, _now);
        var productId = Guid.NewGuid();

        cart.AddItem(productId, "SKU-1", "Product 1", 100m, "INR", 2, 10, _now.AddMinutes(1));

        cart.Items.Should().HaveCount(1);
        var item = cart.Items[0];
        item.ProductId.Should().Be(productId);
        item.Sku.Should().Be("SKU-1");
        item.ProductName.Should().Be("Product 1");
        item.UnitPrice.Should().Be(100m);
        item.Currency.Should().Be("INR");
        item.Quantity.Should().Be(2);
        item.LineTotal.Should().Be(200m);
        cart.Subtotal.Should().Be(200m);
        cart.UpdatedAt.Should().Be(_now.AddMinutes(1));
    }

    [Fact]
    public void AddItem_WhenExistingItem_IncreasesQuantityAndRefreshesPrice()
    {
        var cart = new BullMarket.Domain.Cart.Cart(_customerId, _now);
        var productId = Guid.NewGuid();

        cart.AddItem(productId, "SKU-1", "Product 1", 100m, "INR", 2, 10, _now);
        cart.AddItem(productId, "SKU-1", "Product 1", 120m, "INR", 3, 10, _now.AddMinutes(1));

        cart.Items.Should().HaveCount(1);
        var item = cart.Items[0];
        item.Quantity.Should().Be(5);
        item.UnitPrice.Should().Be(120m);
        item.LineTotal.Should().Be(600m);
        cart.Subtotal.Should().Be(600m);
    }

    [Fact]
    public void UpdateItem_WithValidQuantity_UpdatesItem()
    {
        var cart = new BullMarket.Domain.Cart.Cart(_customerId, _now);
        var productId = Guid.NewGuid();
        cart.AddItem(productId, "SKU-1", "Product 1", 50m, "INR", 1, 10, _now);

        cart.UpdateItem(productId, 4, 10, _now.AddMinutes(2));

        cart.GetItem(productId).Quantity.Should().Be(4);
        cart.Subtotal.Should().Be(200m);
    }

    [Fact]
    public void UpdateItem_ExceedingAvailableQuantity_ThrowsDomainException()
    {
        var cart = new BullMarket.Domain.Cart.Cart(_customerId, _now);
        var productId = Guid.NewGuid();
        cart.AddItem(productId, "SKU-1", "Product 1", 50m, "INR", 1, 10, _now);

        var act = () => cart.UpdateItem(productId, 15, 10, _now);

        act.Should().Throw<DomainException>()
            .WithMessage("*exceeds available quantity*");
    }

    [Fact]
    public void RemoveItem_WhenExists_RemovesFromCart()
    {
        var cart = new BullMarket.Domain.Cart.Cart(_customerId, _now);
        var productId = Guid.NewGuid();
        cart.AddItem(productId, "SKU-1", "Product 1", 50m, "INR", 1, 10, _now);

        cart.RemoveItem(productId, _now.AddMinutes(3));

        cart.Items.Should().BeEmpty();
        cart.Subtotal.Should().Be(0m);
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var cart = new BullMarket.Domain.Cart.Cart(_customerId, _now);
        cart.AddItem(Guid.NewGuid(), "SKU-1", "Product 1", 50m, "INR", 1, 10, _now);
        cart.AddItem(Guid.NewGuid(), "SKU-2", "Product 2", 75m, "INR", 2, 10, _now);

        cart.Clear(_now.AddMinutes(4));

        cart.Items.Should().BeEmpty();
        cart.Subtotal.Should().Be(0m);
    }
}
