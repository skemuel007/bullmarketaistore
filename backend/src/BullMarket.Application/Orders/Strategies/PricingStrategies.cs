using BullMarket.Application.Configuration;

namespace BullMarket.Application.Orders.Strategies;

public interface IShippingStrategy
{
    decimal CalculateShipping(decimal subtotal);
}

public interface ITaxStrategy
{
    decimal CalculateTax(decimal subtotal);
}

public sealed class TieredShippingStrategy(CommerceOptions options) : IShippingStrategy
{
    public decimal CalculateShipping(decimal subtotal) =>
        subtotal >= options.FreeShippingThreshold ? 0m : options.StandardShippingAmount;
}

public sealed class StandardTaxStrategy(CommerceOptions options) : ITaxStrategy
{
    public decimal CalculateTax(decimal subtotal) =>
        decimal.Round(subtotal * options.TaxRate, 2, MidpointRounding.AwayFromZero);
}
