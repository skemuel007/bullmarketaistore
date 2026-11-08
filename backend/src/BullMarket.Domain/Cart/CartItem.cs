using BullMarket.Domain.Common;

namespace BullMarket.Domain.Cart;

public sealed class CartItem
{
    // reference to Cart
    

    private CartItem() {}

    internal CartItem(
        Guid cartId,
        Guid productId,
        string sku,
        string productName,
        decimal unitPrice,
        string currency,
        int quantity
    )
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID is required.", nameof(productId));
        }

        Id = Guid.NewGuid();
        CartId = cartId;
        ProductId = productId;
        Sku = Guard.Required(sku, nameof(sku), 64);
        ProductName = Guard.Required(productName, nameof(productName), 200);
        RefreshPrice(unitPrice, currency);
        SetQuantity(quantity);
    }
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    internal void SetQuantity(int quantity) => Quantity = Guard.Positive(quantity, nameof(quantity));

    internal void RefreshPrice(decimal unitPrice, string currency)
    {
        UnitPrice = Guard.NonNegative(unitPrice, nameof(unitPrice));
        Currency = Guard.Required(currency, nameof(currency), 3);
    }
}