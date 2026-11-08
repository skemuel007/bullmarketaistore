using BullMarket.Api.Filters;
using BullMarket.Application.Abstractions;
using BullMarket.Application.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.Api.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cart/{customerId:guid}").WithTags("Cart");

        group.MapGet("/", GetCart)
            .WithName("GetCart")
            .Produces<CartDto>(StatusCodes.Status200OK);

        group.MapPost("/items", AddItem)
            .WithName("AddCartItem")
            .WithValidation<AddCartItemRequest>()
            .Produces<CartDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPut("/items/{productId:guid}", UpdateItem)
            .WithName("UpdateCartItem")
            .WithValidation<UpdateCartItemRequest>()
            .Produces<CartDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapDelete("/items/{productId:guid}", RemoveItem)
            .WithName("RemoveCartItem")
            .Produces(StatusCodes.Status204NoContent);

        group.MapDelete("/", Clear)
            .WithName("ClearCart")
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }

    private static async Task<Ok<CartDto>> GetCart(
        Guid customerId,
        ICartService cart,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await cart.GetCartAsync(customerId, cancellationToken));

    private static async Task<Ok<CartDto>> AddItem(
        Guid customerId,
        [FromBody] AddCartItemRequest request,
        ICartService cart,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await cart.AddItemAsync(customerId, request, cancellationToken));

    private static async Task<Ok<CartDto>> UpdateItem(
        Guid customerId,
        Guid productId,
        [FromBody] UpdateCartItemRequest request,
        ICartService cart,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await cart.UpdateItemAsync(customerId, productId, request, cancellationToken));

    private static async Task<NoContent> RemoveItem(
        Guid customerId,
        Guid productId,
        ICartService cart,
        CancellationToken cancellationToken)
    {
        await cart.RemoveItemAsync(customerId, productId, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> Clear(
        Guid customerId,
        ICartService cart,
        CancellationToken cancellationToken)
    {
        await cart.ClearAsync(customerId, cancellationToken);
        return TypedResults.NoContent();
    }
}
