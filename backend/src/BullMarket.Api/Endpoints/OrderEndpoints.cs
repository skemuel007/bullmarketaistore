using BullMarket.Api.Filters;
using BullMarket.Application.Abstractions;
using BullMarket.Application.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/orders").WithTags("Orders");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithValidation<CreateOrderRequest>()
            .Produces<OrderDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetOrder)
            .WithName("GetOrder")
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/cancel", CancelOrder)
            .WithName("CancelOrder")
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<CreatedAtRoute<OrderDto>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        IOrderService orders,
        CancellationToken cancellationToken)
    {
        var order = await orders.CreateOrderAsync(request, cancellationToken);
        return TypedResults.CreatedAtRoute(order, "GetOrder", new { id = order.Id });
    }

    private static async Task<Ok<OrderDto>> GetOrder(
        Guid id,
        IOrderService orders,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await orders.GetOrderAsync(id, cancellationToken));

    private static async Task<Ok<OrderDto>> CancelOrder(
        Guid id,
        IOrderService orders,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await orders.CancelOrderAsync(id, cancellationToken));
}
