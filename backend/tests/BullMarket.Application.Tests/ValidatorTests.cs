using BullMarket.Application.Cart;
using BullMarket.Application.Catalog;
using BullMarket.Application.Contracts;
using BullMarket.Application.Orders;
using FluentAssertions;

namespace BullMarket.Application.Tests;

public class ValidatorTests
{
    [Fact]
    public void ProductSearchRequestValidator_ValidatesPageAndPriceRange()
    {
        var validator = new ProductSearchRequestValidator();

        var validRequest = new ProductSearchRequest(MinimumPrice: 10m, MaximumPrice: 100m, Page: 1, PageSize: 20);
        validator.Validate(validRequest).IsValid.Should().BeTrue();

        var invalidPrice = new ProductSearchRequest(MinimumPrice: 200m, MaximumPrice: 100m);
        validator.Validate(invalidPrice).IsValid.Should().BeFalse();

        var invalidPage = new ProductSearchRequest(Page: 0);
        validator.Validate(invalidPage).IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateOrderRequestValidator_ValidatesAddresses()
    {
        var validator = new CreateOrderRequestValidator();

        var valid = new CreateOrderRequest(
            Guid.NewGuid(),
            new AddressDto("John", "Line1", null, "City", "State", "12345", "US", null),
            new AddressDto("John", "Line1", null, "City", "State", "12345", "US", null));

        validator.Validate(valid).IsValid.Should().BeTrue();

        var invalid = new CreateOrderRequest(
            Guid.Empty,
            null!,
            null!);

        validator.Validate(invalid).IsValid.Should().BeFalse();
    }

    [Fact]
    public void CartRequestValidators_ValidateQuantityLimits()
    {
        var addValidator = new AddCartItemRequestValidator();
        var updateValidator = new UpdateCartItemRequestValidator();

        addValidator.Validate(new AddCartItemRequest(Guid.NewGuid(), 1)).IsValid.Should().BeTrue();
        addValidator.Validate(new AddCartItemRequest(Guid.NewGuid(), 0)).IsValid.Should().BeFalse();
        addValidator.Validate(new AddCartItemRequest(Guid.NewGuid(), 101)).IsValid.Should().BeFalse();

        updateValidator.Validate(new UpdateCartItemRequest(5)).IsValid.Should().BeTrue();
        updateValidator.Validate(new UpdateCartItemRequest(0)).IsValid.Should().BeFalse();
    }
}
