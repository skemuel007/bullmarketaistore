namespace BullMarket.Application.Catalog;

public sealed record ProductSummaryDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    string Brand,
    string Category,
    string ShortDescription,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsFeatured,
    bool IsAiReady,
    bool IsLocalLlmReady,
    bool IsDeveloperReady,
    bool IsCreatorReady,
    bool IsGamingReady,
    string? PrimaryImageUrl);

public sealed record ProductDetailsDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    string Brand,
    Guid CategoryId,
    string Category,
    string ShortDescription,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsActive,
    bool IsFeatured,
    ProductTechnicalSpecificationsDto TechnicalSpecifications,
    ProductCapabilitiesDto Capabilities,
    IReadOnlyCollection<string> Tags,
    IReadOnlyCollection<ProductImageDto> Images,
    IReadOnlyCollection<ProductSpecificationDto> Specifications);

public sealed record ProductTechnicalSpecificationsDto(
    string? Cpu,
    int? CpuCores,
    int? RamGb,
    int? MaxRamGb,
    string? Gpu,
    int? VramGb,
    string? Npu,
    int? NpuTops,
    int? StorageGb,
    string? StorageType,
    string? Display,
    decimal? BatteryWh,
    decimal? WeightKg);

public sealed record ProductCapabilitiesDto(
    bool AiReady,
    bool LocalLlmReady,
    bool DeveloperReady,
    bool CreatorReady,
    bool GamingReady,
    bool DockerReady);

public sealed record ProductImageDto(string Url, string Alt, bool IsPrimary, int DisplayOrder);

public sealed record ProductSpecificationDto(string Group, string Name, string Value, int DisplayOrder);

public sealed record CategoryDto(Guid Id, string Name, string Slug, string? Description);

public sealed record ProductSearchResult(
    IReadOnlyCollection<ProductSummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
