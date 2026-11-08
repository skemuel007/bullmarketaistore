using BullMarket.Application.Abstractions;
using BullMarket.Application.Catalog;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BullMarket.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/categories").WithTags("Catalog");

        group.MapGet("/", GetCategories)
            .WithName("GetCategories")
            .Produces<IReadOnlyCollection<CategoryDto>>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<Ok<IReadOnlyCollection<CategoryDto>>> GetCategories(
        IProductService products,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await products.GetCategoriesAsync(cancellationToken));
}
