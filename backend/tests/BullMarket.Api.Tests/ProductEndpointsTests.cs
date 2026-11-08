using System.Net;
using System.Net.Http.Json;
using BullMarket.Application.Abstractions;
using BullMarket.Application.Catalog;
using BullMarket.Application.Common;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BullMarket.Api.Tests;

public class ProductEndpointsTests : IClassFixture<BullMarketApiFactory>
{
    private readonly BullMarketApiFactory _factory;
    private readonly IProductService _productService = Substitute.For<IProductService>();

    public ProductEndpointsTests(BullMarketApiFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateCustomClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => _productService);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkWithProducts()
    {
        var client = CreateCustomClient();
        var fakeResult = new ProductSearchResult([], 0, 1, 20);
        _productService.GetProductsAsync(Arg.Any<ProductSearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(fakeResult);

        var response = await client.GetAsync("/api/products?page=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductSearchResult>();
        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetProductById_WhenNotFound_Returns404ProblemDetails()
    {
        var client = CreateCustomClient();
        var id = Guid.NewGuid();
        _productService.GetProductByIdAsync(id, Arg.Any<CancellationToken>())
            .ThrowsAsync(new NotFoundException("Product", id));

        var response = await client.GetAsync($"/api/products/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Resource not found");
    }

    [Fact]
    public async Task Search_WhenInvalidPriceRange_Returns400ProblemDetails()
    {
        var client = CreateCustomClient();
        _productService.GetProductsAsync(Arg.Any<ProductSearchRequest>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ApplicationValidationException("The price range is invalid."));

        var response = await client.GetAsync("/api/products/search?minPrice=500&maxPrice=100");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The price range is invalid.");
    }
}
