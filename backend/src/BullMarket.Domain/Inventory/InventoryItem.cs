using BullMarket.Domain.Common;

namespace BullMarket.Domain.Inventory;

public sealed class InventoryItem
{
    private InventoryItem()
    {
    }
    
    public InventoryItem(Guid productId, int quantityOnHand, DateTimeOffset updatedAt)
    {
        
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID is required.", nameof(productId));
        }
        
        Id = Guid.NewGuid();
        ProductId = productId;
        QuantityOnHand = quantityOnHand;
        UpdatedAt = updatedAt;
    }
    
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;
    public DateTimeOffset UpdatedAt { get; private set; }
    public uint Version { get; private set; }
    
    public bool IsAvailable(int quantity) => quantity > 0 && QuantityAvailable >= quantity;

    public void Reserve(int quantity, DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        if (!IsAvailable(quantity))
        {
            throw new DomainException("Not enough inventory available to reserve.", nameof(quantity));
        }
        
        QuantityReserved += quantity;
        UpdatedAt = updatedAt;
    }
    
    public void Release(int quantity, DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        if (quantity > QuantityReserved)
        {
            throw new DomainException("Cannot release more inventory than is reserved.", nameof(quantity));
        }
        
        QuantityReserved -= quantity;
        UpdatedAt = updatedAt;
    }
    
    public void CommitReservation(int quantity, DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        if (quantity > QuantityReserved)
        {
            throw new DomainException("Cannot commit more inventory than is reserved.", nameof(quantity));
        }
        
        QuantityReserved -= quantity;
        QuantityOnHand -= quantity;
        UpdatedAt = updatedAt;
    }
    
    public void Reduce(int quantity, DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        if (quantity > QuantityAvailable)
        {
            throw new DomainException("Cannot reduce more inventory than is available.", nameof(quantity));
        }
        
        QuantityOnHand -= quantity;
        UpdatedAt = updatedAt;
    }
    
    public void Restock(int quantity, DateTimeOffset updatedAt)
    {
        Guard.Positive(quantity, nameof(quantity));
        
        QuantityOnHand += quantity;
        UpdatedAt = updatedAt;
    }
}