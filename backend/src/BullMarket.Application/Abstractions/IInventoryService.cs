using BullMarket.Application.Contracts;

namespace BullMarket.Application.Abstractions;

public interface IInventoryService
{
    Task<InventoryDto> GetInventoryAsync(Guid productId, CancellationToken cancellationToken);

    Task<InventoryAvailabilityDto> CheckAvailabilityAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken);
}
