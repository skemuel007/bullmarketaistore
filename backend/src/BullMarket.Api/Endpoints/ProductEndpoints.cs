using BullMarket.Application.Abstractions;
using BullMarket.Application.Catalog;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products").WithTags("Catalog");

        group.MapGet("/", GetProducts)
            .WithName("GetProducts")
            .Produces<ProductSearchResult>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetProduct)
            .WithName("GetProduct")
            .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/search", Search)
            .WithName("SearchProducts")
            .Produces<ProductSearchResult>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<Ok<ProductSearchResult>> GetProducts(
        IProductService products,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) =>
        TypedResults.Ok(await products.GetProductsAsync(
            new ProductSearchRequest(Page: page, PageSize: pageSize),
            cancellationToken));

    private static async Task<Ok<ProductDetailsDto>> GetProduct(
        Guid id,
        IProductService products,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await products.GetProductByIdAsync(id, cancellationToken));

    private static async Task<Ok<ProductSearchResult>> Search(
        IProductService products,
        CancellationToken cancellationToken,
        [FromQuery] string? keyword = null,
        [FromQuery] string? category = null,
        [FromQuery] string? brand = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? ramMin = null,
        [FromQuery] int? vramMin = null,
        [FromQuery] bool? aiReady = null,
        [FromQuery] bool? localLlmReady = null,
        [FromQuery] bool? developerReady = null,
        [FromQuery] bool? creatorReady = null,
        [FromQuery] bool? gamingReady = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) =>
        TypedResults.Ok(await products.GetProductsAsync(
            new ProductSearchRequest(
                keyword, category, brand, minPrice, maxPrice, ramMin, vramMin,
                aiReady, localLlmReady, developerReady, creatorReady, gamingReady, page, pageSize),
            cancellationToken));
}
