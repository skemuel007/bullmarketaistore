using BullMarket.Application.Abstractions;
using BullMarket.Application.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customers").WithTags("Customers");

        group.MapGet("/{id:guid}", GetCustomer)
            .WithName("GetCustomer")
            .Produces<CustomerDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{customerId:guid}/orders", GetOrders)
            .WithName("GetCustomerOrders")
            .Produces<IReadOnlyCollection<OrderDto>>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<Ok<CustomerDto>> GetCustomer(
        Guid id,
        ICustomerService customers,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await customers.GetCustomerAsync(id, cancellationToken));

    private static async Task<Ok<IReadOnlyCollection<OrderDto>>> GetOrders(
        Guid customerId,
        IOrderService orders,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await orders.GetOrdersByCustomerAsync(customerId, cancellationToken));
}
