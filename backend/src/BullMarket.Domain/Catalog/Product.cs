using BullMarket.Domain.Common;

public sealed class Product
{
    private List<ProductImage> _images = new List<ProductImage>();
    private List<ProductSpecification> _specifications = new List<ProductSpecification>();
    private List<ProductTags> _tags = new List<ProductTags>();

    private Product() {}
    public Product(
        string sku,
        string name,
        string slug,
        string brand,
        Guid categoryId,
        string shortDescription,
        string description,
        decimal price,
        string currency,
        DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        Sku = Guard.Required(sku, nameof(sku), 64).ToUpperInvariant();
        Name = Guard.Required(name, nameof(name), 200);
        Slug = Guard.Required(slug, nameof(slug), 220).ToLowerInvariant();
        Brand = Guard.Required(brand, nameof(brand), 100);
        CategoryId = categoryId != Guid.Empty ? categoryId : throw new DomainException("Category ID is required.", nameof(categoryId));
        ShortDescription = Guard.Required(shortDescription, nameof(shortDescription), 500);
        Description = Guard.Required(description, nameof(description), 5000);
        Price = Guard.NonNegative(price, nameof(price));
        Currency = Guard.Required(currency, nameof(currency), 3);
        IsActive = true;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public string ShortDescription { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsFeatured { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; } 

    public string? Cpu { get; private set; }
    public int? CpuCores { get; private set; }
    public int? RamGb { get; private set; }
    public int? MaxRamGb { get; private set; }
    public string? Gpu { get; private set; }
    public string? Npu { get; private set; }
    public int? VramGb { get; private set; }
    public int? NpuTops { get; private set; }
    public int? StorageGb { get; private set; }
    public string? StorageType { get; private set; }
    public string? Display { get; private set; }
    public decimal? BatteryWh { get; private set; }
    public decimal? WeightKg { get; private set; }

    public bool IsAiReady {get; private set;}
    public bool IsLocalLlmReady {get; private set;}
    public bool IsDeveloperReady {get; private set;}
    public bool IsCreatorReady {get; private set;}
    public bool IsGamingReady {get; private set;}
    public bool IsDockerReady {get; private set;}

    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<ProductSpecification> Specifications => _specifications.AsReadOnly();
    public IReadOnlyCollection<ProductTags> Tags => _tags.AsReadOnly();

    public void SetTechnicalSpecifications(
        ProductTechnicalDetails details, DateTimeOffset updatedAt)
    {
        Cpu = details.Cpu?.Trim();
        CpuCores = details.CpuCores;
        RamGb = details.RamGb;
        MaxRamGb = details.MaxRamGb;
        Gpu = details.Gpu;
        Npu = details.Npu;
        VramGb = details.VramGb;
        NpuTops = details.NpuTops;
        StorageGb = details.StorageGb;
        StorageType = details.StorageType;
        Display = details.Display;
        BatteryWh = details.BatteryWh;
        WeightKg = details.WeightKg;
        UpdatedAt = updatedAt;
    }

    public void SetUsageClassification(ProductUsageClassification classification, DateTimeOffset updatedAt)
    {
        IsAiReady = classification.IsAiReady;
        IsLocalLlmReady = classification.IsLocalLlmReady;
        IsDeveloperReady = classification.IsDeveloperReady;
        IsCreatorReady = classification.IsCreatorReady;
        IsGamingReady = classification.IsGamingReady;
        IsDockerReady = classification.IsDockerReady;
        UpdatedAt = updatedAt;
    }

    public void ChangePrice(decimal newPrice, string currency, DateTimeOffset updatedAt)
    {
        Price = Guard.NonNegative(newPrice, nameof(newPrice));
        Currency = Guard.Required(currency, nameof(currency), 3).ToUpperInvariant();
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    public void SetFeatured(bool isFeatured, DateTimeOffset updatedAt)
    {
        IsFeatured = isFeatured;
        UpdatedAt = updatedAt;
    }

    public void AddImage(string url, string altText, bool isPrimary = false, int displayOrder = 0)
    {
        if (isPrimary)
        {
            foreach (var image in _images)
            {
                image.ClearPrimary();
            }
        }

        _images.Add(new ProductImage(Id, url, altText, isPrimary, displayOrder));
    }

    public void AddSpecification(string group, string name, string value, int displayOrder = 0)
    {
        _specifications.Add(new ProductSpecification(Id, group, name, value, displayOrder));
    }

    public void AddTag(string name)
    {
        var normalizedName = Guard.Required(name, nameof(name), 50).ToLowerInvariant();
        if (_tags.All(t => t.Name != normalizedName))
        {
            _tags.Add(new ProductTags(Id, normalizedName));
        }
        else
        {
            throw new DomainException($"Tag '{normalizedName}' already exists for this product.", nameof(name));
        }
    }
}
