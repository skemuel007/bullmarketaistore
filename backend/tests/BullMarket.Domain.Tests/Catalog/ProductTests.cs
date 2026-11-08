using BullMarket.Domain.Common;
using FluentAssertions;

namespace BullMarket.Domain.Tests.Catalog;

public class ProductTests
{
    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Constructor_InitializesProductWithNormalizedValues()
    {
        var product = new Product(
            "sku-pro-1",
            "Pro Laptop",
            "Pro-Laptop-Slug",
            "BrandX",
            _categoryId,
            "Short description",
            "Detailed description",
            1999.99m,
            "INR",
            _now);

        product.Id.Should().NotBeEmpty();
        product.Sku.Should().Be("SKU-PRO-1"); // upper case
        product.Slug.Should().Be("pro-laptop-slug"); // lower case
        product.Brand.Should().Be("BrandX");
        product.CategoryId.Should().Be(_categoryId);
        product.Price.Should().Be(1999.99m);
        product.Currency.Should().Be("INR");
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void AddImagesSpecificationsAndTags_PopulatesCollections()
    {
        var product = new Product(
            "SKU-1", "Name", "slug", "Brand", _categoryId,
            "Short", "Description", 100m, "INR", _now);

        product.AddImage("http://example.com/img1.png", "Image 1", true, 0);
        product.AddSpecification("Performance", "RAM", "32GB", 1);
        product.AddTag("AI");

        product.Images.Should().HaveCount(1);
        product.Specifications.Should().HaveCount(1);
        product.Tags.Should().HaveCount(1);
        product.Tags.First().Name.Should().Be("ai"); // lowercase normalized
    }

    [Fact]
    public void SetTechnicalSpecifications_SetsProperties()
    {
        var product = new Product(
            "SKU-1", "Name", "slug", "Brand", _categoryId,
            "Short", "Description", 100m, "INR", _now);

        var details = new ProductTechnicalDetails(
            cpu: "Intel i9",
            cpuCores: 16,
            ramGb: 32,
            maxRamGb: 64,
            gpu: "RTX 4080",
            npu: "Intel AI",
            vramGb: 12,
            npuTops: 45,
            storageGb: 1024,
            storageType: "NVMe SSD",
            display: "16-inch 4K",
            batteryWh: 90m,
            weightKg: 2.1m);

        product.SetTechnicalSpecifications(details, _now.AddMinutes(1));

        product.Cpu.Should().Be("Intel i9");
        product.CpuCores.Should().Be(16);
        product.RamGb.Should().Be(32);
        product.MaxRamGb.Should().Be(64);
        product.Gpu.Should().Be("RTX 4080");
        product.StorageGb.Should().Be(1024);
        product.UpdatedAt.Should().Be(_now.AddMinutes(1));
    }
}
