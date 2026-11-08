using BullMarket.Application.Abstractions;
using BullMarket.Application.Catalog;
using BullMarket.Application.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BullMarket.Application.Tests.Catalog;

public class ProductServiceTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly IInventoryRepository _inventory = Substitute.For<IInventoryRepository>();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_products, _inventory, NullLogger<ProductService>.Instance);
    }

    [Fact]
    public async Task GetProductsAsync_WithInvalidPage_ThrowsApplicationValidationException()
    {
        var request = new ProductSearchRequest(Page: 0);

        var act = () => _sut.GetProductsAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("*Page must be at least 1*");
    }

    [Fact]
    public async Task GetProductsAsync_WhenValid_ReturnsEnrichedProductSearchResult()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category("Laptops", "laptops", "Laptops desc");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        var product = new Product("SKU-1", "Laptop Pro", "laptop-pro", "BrandX", categoryId, "Short", "Long", 1500m, "INR", DateTimeOffset.UtcNow);

        _products.SearchAsync(Arg.Any<ProductSearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(([product], 1));
        _products.GetCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns([category]);
        _inventory.GetAvailableQuantitiesAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, int> { [product.Id] = 5 });

        var request = new ProductSearchRequest();
        var result = await _sut.GetProductsAsync(request, CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        var item = result.Items.First();
        item.Id.Should().Be(product.Id);
        item.Category.Should().Be("Laptops");
        item.StockQuantity.Should().Be(5);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products.GetByIdAsync(productId, false, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var act = () => _sut.GetProductByIdAsync(productId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
