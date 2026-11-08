using BullMarket.Domain.Orders;

namespace BullMarket.Application.Contracts;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    AddressDto ShippingAddress,
    AddressDto BillingAddress);

public sealed record OrderItemDto(
    Guid ProductId,
    string Sku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    DateTimeOffset OrderDate,
    IReadOnlyCollection<OrderItemDto> Items,
    decimal Subtotal,
    decimal ShippingAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    string Currency,
    OrderStatus OrderStatus,
    PaymentStatus PaymentStatus,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    DateTimeOffset? CancelledAt);
