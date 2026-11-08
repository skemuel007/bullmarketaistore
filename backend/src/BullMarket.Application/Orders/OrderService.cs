using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Application.Configuration;
using BullMarket.Application.Contracts;
using BullMarket.Application.Orders.Strategies;
using BullMarket.Domain.Cart;
using BullMarket.Domain.Customers;
using BullMarket.Domain.Inventory;
using BullMarket.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BullMarket.Application.Orders;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICartRepository _carts;
    private readonly ICustomerRepository _customers;
    private readonly IInventoryRepository _inventory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IShippingStrategy _shippingStrategy;
    private readonly ITaxStrategy _taxStrategy;
    private readonly IDomainEventDispatcher? _eventDispatcher;
    private readonly ILogger<OrderService> _logger;

    [ActivatorUtilitiesConstructor]
    public OrderService(
        IOrderRepository orders,
        ICartRepository carts,
        ICustomerRepository customers,
        IInventoryRepository inventory,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IShippingStrategy shippingStrategy,
        ITaxStrategy taxStrategy,
        IDomainEventDispatcher? eventDispatcher,
        ILogger<OrderService> logger)
    {
        _orders = orders;
        _carts = carts;
        _customers = customers;
        _inventory = inventory;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _shippingStrategy = shippingStrategy;
        _taxStrategy = taxStrategy;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken) =>
        _unitOfWork.ExecuteInTransactionAsync(
            innerCancellationToken => CreateOrderCoreAsync(request, innerCancellationToken),
            cancellationToken);

    private async Task<OrderDto> CreateOrderCoreAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        _ = await _customers.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.CustomerId);
        var cart = await _carts.GetByCustomerIdAsync(request.CustomerId, true, cancellationToken)
            ?? throw new NotFoundException(nameof(Cart), request.CustomerId);
        if (cart.Items.Count == 0)
        {
            throw new ApplicationValidationException("An order cannot be created from an empty cart.");
        }

        var now = _timeProvider.GetUtcNow();
        foreach (var cartItem in cart.Items)
        {
            var inventoryItem = await _inventory.GetByProductIdAsync(cartItem.ProductId, true, cancellationToken)
                ?? throw new NotFoundException(nameof(InventoryItem), cartItem.ProductId);
            inventoryItem.Reduce(cartItem.Quantity, now);
        }

        var shipping = _shippingStrategy.CalculateShipping(cart.Subtotal);
        var tax = _taxStrategy.CalculateTax(cart.Subtotal);
        var order = Order.Create(
            CreateOrderNumber(now),
            request.CustomerId,
            cart.Items.Select(item => new OrderLine(
                item.ProductId, item.Sku, item.ProductName, item.UnitPrice, item.Quantity)),
            request.ShippingAddress.ToDomain(),
            request.BillingAddress.ToDomain(),
            shipping,
            tax,
            cart.Items.Select(item => item.Currency).Distinct(StringComparer.OrdinalIgnoreCase).Single(),
            now);

        _orders.Add(order);
        cart.Clear(now);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_eventDispatcher is not null && order.DomainEvents.Count > 0)
        {
            await _eventDispatcher.DispatchAsync(order.DomainEvents, cancellationToken);
            order.ClearDomainEvents();
        }

        _logger.LogInformation(
            "Created order {OrderId} with number {OrderNumber} for customer {CustomerId}; total {TotalAmount} {Currency}",
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.TotalAmount,
            order.Currency);

        return order.ToDto();
    }

    public async Task<OrderDto> GetOrderAsync(Guid id, CancellationToken cancellationToken) =>
        (await GetOrderEntityAsync(id, cancellationToken)).ToDto();

    public async Task<IReadOnlyCollection<OrderDto>> GetOrdersByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        _ = await _customers.GetByIdAsync(customerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), customerId);
        return (await _orders.GetByCustomerIdAsync(customerId, cancellationToken))
            .Select(order => order.ToDto())
            .ToArray();
    }

    public Task<OrderDto> CancelOrderAsync(Guid id, CancellationToken cancellationToken) =>
        _unitOfWork.ExecuteInTransactionAsync(
            innerCancellationToken => CancelOrderCoreAsync(id, innerCancellationToken),
            cancellationToken);

    private async Task<OrderDto> CancelOrderCoreAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await GetOrderEntityAsync(id, cancellationToken);
        var now = _timeProvider.GetUtcNow();
        order.Cancel(now);

        foreach (var orderItem in order.Items)
        {
            var inventoryItem = await _inventory.GetByProductIdAsync(orderItem.ProductId, true, cancellationToken)
                ?? throw new NotFoundException(nameof(InventoryItem), orderItem.ProductId);
            inventoryItem.Restock(orderItem.Quantity, now);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_eventDispatcher is not null && order.DomainEvents.Count > 0)
        {
            await _eventDispatcher.DispatchAsync(order.DomainEvents, cancellationToken);
            order.ClearDomainEvents();
        }

        _logger.LogInformation(
            "Cancelled order {OrderId} for customer {CustomerId} and restored {ItemCount} inventory lines",
            order.Id,
            order.CustomerId,
            order.Items.Count);

        return order.ToDto();
    }

    private async Task<Order> GetOrderEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await _orders.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException(nameof(Order), id);

    private static string CreateOrderNumber(DateTimeOffset now)
    {
        var sortableId = Guid.CreateVersion7(now).ToString("N")[..16].ToUpperInvariant();
        return $"BM-{now:yyyyMMdd}-{sortableId}";
    }
}
