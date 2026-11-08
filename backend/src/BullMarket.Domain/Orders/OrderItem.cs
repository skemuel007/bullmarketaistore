using BullMarket.Domain.Common;

namespace BullMarket.Domain.Orders;

public sealed class OrderItem
{
    private OrderItem() {}
    
    public OrderItem(
        Guid orderId,
        OrderLine line
    )
    {
        if (line.ProductId == Guid.Empty)
        {
            throw new DomainException("Product ID is required.", nameof(line.ProductId));
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = line.ProductId;
        Sku = Guard.Required(line.Sku, nameof(line.Sku), 64);
        ProductName = Guard.Required(line.ProductName, nameof(line.ProductName), 200);
        UnitPrice = Guard.NonNegative(line.UnitPrice, nameof(line.UnitPrice));
        Quantity = Guard.Positive(line.Quantity, nameof(line.Quantity));
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
