using BullMarket.Domain.Common;

public sealed class ProductImage
{
    private ProductImage() {}

    internal ProductImage(Guid productId, string url, string altText, bool isPrimary, int displayOrder)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Url = Guard.Required(url, nameof(url), 500);
        AltText = Guard.Required(altText, nameof(altText), 200);
        IsPrimary = isPrimary;
        DisplayOrder = displayOrder;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string AltText { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public int DisplayOrder { get; private set; }

    internal void ClearPrimary()
    {
        IsPrimary = false;
    }
}
