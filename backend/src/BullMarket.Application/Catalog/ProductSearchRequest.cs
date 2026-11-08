namespace BullMarket.Application.Catalog;

public sealed record ProductSearchRequest(
    string? Keyword = null,
    string? Category = null,
    string? Brand = null,
    decimal? MinimumPrice = null,
    decimal? MaximumPrice = null,
    int? MinimumRamGb = null,
    int? MinimumVramGb = null,
    bool? AiReady = null,
    bool? LocalLlmReady = null,
    bool? DeveloperReady = null,
    bool? CreatorReady = null,
    bool? GamingReady = null,
    int Page = 1,
    int PageSize = 20);
