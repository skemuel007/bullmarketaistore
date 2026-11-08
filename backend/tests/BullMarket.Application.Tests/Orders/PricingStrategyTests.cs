using BullMarket.Application.Configuration;
using BullMarket.Application.Orders.Strategies;
using FluentAssertions;

namespace BullMarket.Application.Tests.Orders;

public class PricingStrategyTests
{
    private readonly CommerceOptions _options = new()
    {
        FreeShippingThreshold = 50000m,
        StandardShippingAmount = 500m,
        TaxRate = 0.18m
    };

    [Fact]
    public void TieredShippingStrategy_BelowThreshold_AppliesStandardShipping()
    {
        var strategy = new TieredShippingStrategy(_options);

        var shipping = strategy.CalculateShipping(49999m);

        shipping.Should().Be(500m);
    }

    [Fact]
    public void TieredShippingStrategy_AtOrAboveThreshold_AppliesFreeShipping()
    {
        var strategy = new TieredShippingStrategy(_options);

        strategy.CalculateShipping(50000m).Should().Be(0m);
        strategy.CalculateShipping(100000m).Should().Be(0m);
    }

    [Fact]
    public void StandardTaxStrategy_CalculatesRoundedTax()
    {
        var strategy = new StandardTaxStrategy(_options);

        // 1000 * 0.18 = 180.00
        strategy.CalculateTax(1000m).Should().Be(180m);
        // 100.55 * 0.18 = 18.099 -> rounded 18.10
        strategy.CalculateTax(100.55m).Should().Be(18.10m);
    }
}
