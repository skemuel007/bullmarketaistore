using BullMarket.Application.Catalog.Specifications;
using FluentAssertions;

namespace BullMarket.Application.Tests.Catalog;

public class SpecificationTests
{
    private readonly Guid _catId = Guid.NewGuid();

    private Product CreateProduct(decimal price, bool isActive = true, bool aiReady = false)
    {
        var p = new Product("SKU-X", "Name", "slug", "Brand", _catId, "Short", "Desc", price, "INR", DateTimeOffset.UtcNow);
        if (!isActive) p.Deactivate(DateTimeOffset.UtcNow);
        if (aiReady)
        {
            p.SetUsageClassification(new ProductUsageClassification(
                IsAiReady: true,
                IsLocalLlmReady: false,
                IsDeveloperReady: false,
                IsCreatorReady: false,
                IsGamingReady: false,
                IsDockerReady: false), DateTimeOffset.UtcNow);
        }
        return p;
    }

    [Fact]
    public void ActiveProductSpecification_FiltersActiveProducts()
    {
        var spec = new ActiveProductSpecification();

        spec.IsSatisfiedBy(CreateProduct(100m, isActive: true)).Should().BeTrue();
        spec.IsSatisfiedBy(CreateProduct(100m, isActive: false)).Should().BeFalse();
    }

    [Fact]
    public void ProductPriceRangeSpecification_FiltersPriceBoundaries()
    {
        var spec = new ProductPriceRangeSpecification(50m, 200m);

        spec.IsSatisfiedBy(CreateProduct(50m)).Should().BeTrue();
        spec.IsSatisfiedBy(CreateProduct(150m)).Should().BeTrue();
        spec.IsSatisfiedBy(CreateProduct(200m)).Should().BeTrue();
        spec.IsSatisfiedBy(CreateProduct(49m)).Should().BeFalse();
        spec.IsSatisfiedBy(CreateProduct(201m)).Should().BeFalse();
    }

    [Fact]
    public void ComposedSpecification_CombinesWithAndOrNot()
    {
        var activeSpec = new ActiveProductSpecification();
        var priceSpec = new ProductPriceRangeSpecification(100m, 500m);

        var combinedAnd = activeSpec.And(priceSpec);
        var combinedOr = activeSpec.Or(priceSpec);
        var notActive = activeSpec.Not();

        combinedAnd.IsSatisfiedBy(CreateProduct(200m, isActive: true)).Should().BeTrue();
        combinedAnd.IsSatisfiedBy(CreateProduct(200m, isActive: false)).Should().BeFalse();
        combinedAnd.IsSatisfiedBy(CreateProduct(50m, isActive: true)).Should().BeFalse();

        combinedOr.IsSatisfiedBy(CreateProduct(50m, isActive: true)).Should().BeTrue();
        combinedOr.IsSatisfiedBy(CreateProduct(200m, isActive: false)).Should().BeTrue();

        notActive.IsSatisfiedBy(CreateProduct(100m, isActive: false)).Should().BeTrue();
        notActive.IsSatisfiedBy(CreateProduct(100m, isActive: true)).Should().BeFalse();
    }
}
