using BullMarket.Application.Contracts;

namespace BullMarket.Application.Abstractions;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);

    Task<OrderDto> GetOrderAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrderDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken);

    Task<OrderDto> CancelOrderAsync(Guid id, CancellationToken cancellationToken);
}
