using System.Net;
using System.Net.Http.Json;
using BullMarket.Application.Abstractions;
using BullMarket.Application.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace BullMarket.Api.Tests;

public class CartEndpointsTests : IClassFixture<BullMarketApiFactory>
{
    private readonly BullMarketApiFactory _factory;
    private readonly ICartService _cartService = Substitute.For<ICartService>();

    public CartEndpointsTests(BullMarketApiFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateCustomClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => _cartService);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetCart_ReturnsOk()
    {
        var client = CreateCustomClient();
        var customerId = Guid.NewGuid();
        var fakeCart = new CartDto(Guid.NewGuid(), customerId, [], 0m, "INR");
        _cartService.GetCartAsync(customerId, Arg.Any<CancellationToken>()).Returns(fakeCart);

        var response = await client.GetAsync($"/api/cart/{customerId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        cart.Should().NotBeNull();
        cart!.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public async Task AddItem_WhenQuantityIsZero_ReturnsBadRequestValidationProblem()
    {
        var client = CreateCustomClient();
        var customerId = Guid.NewGuid();
        var request = new AddCartItemRequest(Guid.NewGuid(), 0); // 0 is invalid, validator expects >= 1

        var response = await client.PostAsJsonAsync($"/api/cart/{customerId}/items", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Quantity");
    }

    [Fact]
    public async Task AddItem_WhenValid_ReturnsOk()
    {
        var client = CreateCustomClient();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new AddCartItemRequest(productId, 2);
        var fakeCart = new CartDto(
            Guid.NewGuid(),
            customerId,
            [new CartItemDto(productId, "SKU", "Prod", 100m, "INR", 2, 200m)],
            200m,
            "INR");

        _cartService.AddItemAsync(customerId, Arg.Any<AddCartItemRequest>(), Arg.Any<CancellationToken>())
            .Returns(fakeCart);

        var response = await client.PostAsJsonAsync($"/api/cart/{customerId}/items", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CartDto>();
        result!.Subtotal.Should().Be(200m);
    }

    [Fact]
    public async Task DeleteItem_ReturnsNoContent()
    {
        var client = CreateCustomClient();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var response = await client.DeleteAsync($"/api/cart/{customerId}/items/{productId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await _cartService.Received(1).RemoveItemAsync(customerId, productId, Arg.Any<CancellationToken>());
    }
}
