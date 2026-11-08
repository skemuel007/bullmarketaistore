using BullMarket.Domain.Common;
using BullMarket.Domain.Inventory;
using FluentAssertions;

namespace BullMarket.Domain.Tests.Inventory;

public class InventoryItemTests
{
    private readonly Guid _productId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Constructor_InitializesInventoryItem()
    {
        var item = new InventoryItem(_productId, 20, _now);

        item.Id.Should().NotBeEmpty();
        item.ProductId.Should().Be(_productId);
        item.QuantityOnHand.Should().Be(20);
        item.QuantityReserved.Should().Be(0);
        item.QuantityAvailable.Should().Be(20);
        item.UpdatedAt.Should().Be(_now);
    }

    [Fact]
    public void Reserve_WhenAvailable_IncreasesQuantityReserved()
    {
        var item = new InventoryItem(_productId, 20, _now);

        item.Reserve(5, _now.AddMinutes(1));

        item.QuantityReserved.Should().Be(5);
        item.QuantityAvailable.Should().Be(15);
        item.QuantityOnHand.Should().Be(20);
    }

    [Fact]
    public void Reserve_WhenExceedingAvailable_ThrowsDomainException()
    {
        var item = new InventoryItem(_productId, 5, _now);

        var act = () => item.Reserve(10, _now);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Release_DecreasesReservedQuantity()
    {
        var item = new InventoryItem(_productId, 20, _now);
        item.Reserve(10, _now);

        item.Release(4, _now.AddMinutes(2));

        item.QuantityReserved.Should().Be(6);
        item.QuantityAvailable.Should().Be(14);
    }

    [Fact]
    public void CommitReservation_DecreasesBothReservedAndOnHand()
    {
        var item = new InventoryItem(_productId, 20, _now);
        item.Reserve(10, _now);

        item.CommitReservation(6, _now.AddMinutes(2));

        item.QuantityReserved.Should().Be(4);
        item.QuantityOnHand.Should().Be(14);
        item.QuantityAvailable.Should().Be(10);
    }

    [Fact]
    public void Reduce_WhenAvailable_DecreasesOnHand()
    {
        var item = new InventoryItem(_productId, 20, _now);

        item.Reduce(5, _now.AddMinutes(1));

        item.QuantityOnHand.Should().Be(15);
        item.QuantityAvailable.Should().Be(15);
    }

    [Fact]
    public void Restock_IncreasesOnHand()
    {
        var item = new InventoryItem(_productId, 10, _now);

        item.Restock(15, _now.AddMinutes(1));

        item.QuantityOnHand.Should().Be(25);
        item.QuantityAvailable.Should().Be(25);
    }
}
