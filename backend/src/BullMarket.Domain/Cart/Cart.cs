using BullMarket.Domain.Common;

namespace BullMarket.Domain.Cart;

public sealed class Cart
{
    // My Cart can have multiple items, so we use a list to store them
    private readonly List<CartItem> _items = [];

    private Cart() {}

    public Cart(Guid customerId, DateTimeOffset createdAt)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer ID .", nameof(customerId));
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set;}
    public Guid CustomerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public decimal Subtotal => _items.Sum(item => item.LineTotal);

    public void AddItem(Guid productId,
        string sku,
        string productName,
        decimal unitPrice,
        string currency,
        int quantity,
        int availableQuantity,
        DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        Guard.NonNegative(availableQuantity, nameof(availableQuantity));
      
        var existingItem = _items.SingleOrDefault(item => item.ProductId == productId);
        var requestedTotal = quantity + (existingItem?.Quantity ?? 0);

        if (existingItem != null)
        {
            existingItem.SetQuantity(existingItem.Quantity + quantity);
            existingItem.RefreshPrice(unitPrice, currency);
        }
        else
        {
            var newItem = new CartItem(Id, productId, sku, productName, unitPrice, currency, quantity);
            _items.Add(newItem);
        }

        UpdatedAt = updatedAt;
    }

    public void UpdateItem(Guid productId, int quantity, int availableQuantity, DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        EnsureAvailableQuantity(quantity, availableQuantity); 
        GetItem(productId).SetQuantity(quantity);

        UpdatedAt = updatedAt;
    }

    public void RemoveItem(Guid productId, DateTimeOffset updatedAt)
    {
        var item = GetItem(productId);
        _items.Remove(item);

        UpdatedAt = updatedAt;
    }

    public void Clear(DateTimeOffset updatedAt)
    {
        _items.Clear();
        UpdatedAt = updatedAt;
    }

    public CartItem GetItem(Guid productId)
    {
        var item = _items.SingleOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new DomainException($"Item with Product ID {productId} not found in the cart.", nameof(productId));
        }
        return item;
    }

    public static void EnsureAvailableQuantity(int requestedQuantity, int availableQuantity)
    {
        if (requestedQuantity > availableQuantity)
        {
            throw new DomainException($"Requested quantity {requestedQuantity} exceeds available quantity {availableQuantity}.", nameof(requestedQuantity));
        }
    }
}
