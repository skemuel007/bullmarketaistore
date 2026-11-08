using BullMarket.Application.Abstractions;
using BullMarket.Application.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.Api.Endpoints;

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory/{productId:guid}").WithTags("Inventory");

        group.MapGet("/", GetInventory)
            .WithName("GetInventory")
            .Produces<InventoryDto>(StatusCodes.Status200OK);

        group.MapGet("/availability", CheckAvailability)
            .WithName("CheckInventoryAvailability")
            .Produces<InventoryAvailabilityDto>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<Ok<InventoryDto>> GetInventory(
        Guid productId,
        IInventoryService inventory,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await inventory.GetInventoryAsync(productId, cancellationToken));

    private static async Task<Ok<InventoryAvailabilityDto>> CheckAvailability(
        Guid productId,
        IInventoryService inventory,
        CancellationToken cancellationToken,
        [FromQuery] int quantity = 1) =>
        TypedResults.Ok(await inventory.CheckAvailabilityAsync(productId, quantity, cancellationToken));
}
