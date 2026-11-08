using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Application.Contracts;
using BullMarket.Domain.Inventory;
using Microsoft.Extensions.Logging;

namespace BullMarket.Application.Inventory;

public sealed class InventoryService(IInventoryRepository inventory, ILogger<InventoryService> logger) : IInventoryService
{
    public async Task<InventoryDto> GetInventoryAsync(Guid productId, CancellationToken cancellationToken)
    {
        var item = await inventory.GetByProductIdAsync(productId, false, cancellationToken)
            ?? throw new NotFoundException(nameof(InventoryItem), productId);
        return ToDto(item);
    }

    public async Task<InventoryAvailabilityDto> CheckAvailabilityAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new ApplicationValidationException("Quantity must be greater than zero.");
        }

        var item = await inventory.GetByProductIdAsync(productId, false, cancellationToken)
            ?? throw new NotFoundException(nameof(InventoryItem), productId);
        logger.LogDebug(
            "Inventory availability checked for product {ProductId}; requested {RequestedQuantity}, available {AvailableQuantity}",
            productId,
            quantity,
            item.QuantityAvailable);
        return new InventoryAvailabilityDto(productId, quantity, item.IsAvailable(quantity), item.QuantityAvailable);
    }

    private static InventoryDto ToDto(InventoryItem item) => new(
        item.ProductId,
        item.QuantityOnHand,
        item.QuantityReserved,
        item.QuantityAvailable,
        item.UpdatedAt);
}
