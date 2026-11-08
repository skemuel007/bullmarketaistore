using BullMarket.Application.Contracts;

namespace BullMarket.Application.Abstractions;

public interface ICartService
{
    Task<CartDto> GetCartAsync(Guid customerId, CancellationToken cancellationToken);

    Task<CartDto> AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken cancellationToken);

    Task<CartDto> UpdateItemAsync(
        Guid customerId,
        Guid productId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken);

    Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken);

    Task ClearAsync(Guid customerId, CancellationToken cancellationToken);
}
