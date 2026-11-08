using BullMarket.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BullMarket.Infrastructure.Persistence.Configurations;

internal static class AddressMapping
{
    /// <summary>Maps <see cref="Address"/> columns using the given prefix, e.g. "shipping_line1".</summary>
    public static void MapAddress(this OwnedNavigationBuilder<Customer, Address> builder, string prefix) => Map(builder, prefix);

    public static void MapAddress(this OwnedNavigationBuilder<BullMarket.Domain.Orders.Order, Address> builder, string prefix) => Map(builder, prefix);

    private static void Map<TOwner>(OwnedNavigationBuilder<TOwner, Address> builder, string prefix) where TOwner : class
    {
        builder.Property(address => address.RecipientName).HasColumnName($"{prefix}_recipient_name").HasMaxLength(150);
        builder.Property(address => address.Line1).HasColumnName($"{prefix}_line1").HasMaxLength(200);
        builder.Property(address => address.Line2).HasColumnName($"{prefix}_line2").HasMaxLength(200);
        builder.Property(address => address.City).HasColumnName($"{prefix}_city").HasMaxLength(100);
        builder.Property(address => address.StateOrProvince).HasColumnName($"{prefix}_state_or_province").HasMaxLength(100);
        builder.Property(address => address.PostalCode).HasColumnName($"{prefix}_postal_code").HasMaxLength(20);
        builder.Property(address => address.CountryCode).HasColumnName($"{prefix}_country_code").HasMaxLength(2);
        builder.Property(address => address.Phone).HasColumnName($"{prefix}_phone").HasMaxLength(30);
    }
}
