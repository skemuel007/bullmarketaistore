using BullMarket.Domain.Common;

public sealed class Category
{
    private Category()
    {
        
    }

    public Category(string name, string slug, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = Guard.Required(name, nameof(name), 100);
        Slug = Guard.Required(slug, nameof(slug), 100);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
}
