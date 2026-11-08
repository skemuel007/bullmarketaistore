using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Application.Inventory;
using BullMarket.Domain.Inventory;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BullMarket.Application.Tests.Inventory;

public class InventoryServiceTests
{
    private readonly IInventoryRepository _inventory = Substitute.For<IInventoryRepository>();
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _sut = new InventoryService(_inventory, NullLogger<InventoryService>.Instance);
    }

    [Fact]
    public async Task GetInventoryAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _inventory.GetByProductIdAsync(productId, false, Arg.Any<CancellationToken>()).Returns((InventoryItem?)null);

        var act = () => _sut.GetInventoryAsync(productId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CheckAvailabilityAsync_WhenAvailable_ReturnsTrue()
    {
        var productId = Guid.NewGuid();
        var item = new InventoryItem(productId, 10, DateTimeOffset.UtcNow);
        _inventory.GetByProductIdAsync(productId, false, Arg.Any<CancellationToken>()).Returns(item);

        var result = await _sut.CheckAvailabilityAsync(productId, 5, CancellationToken.None);

        result.IsAvailable.Should().BeTrue();
        result.AvailableQuantity.Should().Be(10);
        result.RequestedQuantity.Should().Be(5);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_WhenQuantityNonPositive_ThrowsApplicationValidationException()
    {
        var productId = Guid.NewGuid();

        var act = () => _sut.CheckAvailabilityAsync(productId, 0, CancellationToken.None);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("*greater than zero*");
    }
}
