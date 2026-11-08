using BullMarket.Domain.Common;

public sealed class ProductTags
{
    private ProductTags() {}

    internal ProductTags(Guid productId, string name)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Name = Guard.Required(name, nameof(name), 50).ToLowerInvariant();
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
}
