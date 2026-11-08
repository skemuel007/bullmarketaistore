using BullMarket.Domain.Inventory;

namespace BullMarket.Application.Abstractions;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(Guid productId, bool trackChanges, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> GetAvailableQuantitiesAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken);
}
