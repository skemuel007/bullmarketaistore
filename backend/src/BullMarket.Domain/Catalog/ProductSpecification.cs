using BullMarket.Domain.Common;

public sealed class ProductSpecification
{
    private ProductSpecification() {}

    internal ProductSpecification(Guid productId, string group, string name, string value, int displayOrder)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Group = Guard.Required(group, nameof(group), 100);
        Name = Guard.Required(name, nameof(name), 100);
        Value = Guard.Required(value, nameof(value), 500);
        DisplayOrder = displayOrder;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Group { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
}
