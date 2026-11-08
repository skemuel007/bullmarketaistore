using BullMarket.Domain.Customers;
using BullMarket.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BullMarket.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(order => order.Id);
        builder.Property(order => order.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(order => order.ShippingAmount).HasPrecision(18, 2);
        builder.Property(order => order.TaxAmount).HasPrecision(18, 2);
        builder.Property(order => order.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(order => order.OrderStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(order => order.PaymentStatus).HasConversion<string>().HasMaxLength(30);
        builder.Ignore(order => order.Subtotal);
        builder.Ignore(order => order.TotalAmount);
        builder.Ignore(order => order.DomainEvents);
        builder.HasIndex(order => order.OrderNumber).IsUnique();
        builder.HasIndex(order => new { order.CustomerId, order.OrderDate });

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // builder.Navigation(order => order.ShippingAddress).IsRequired();
        // builder.Navigation(order => order.BillingAddress).IsRequired();

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(order => order.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        // Version concurrency handling is configured in the DbContext (xmin on PostgreSQL).
        
        builder.OwnsOne(order => order.ShippingAddress, address => address.MapAddress("shipping"));
        builder.OwnsOne(order => order.BillingAddress, address => address.MapAddress("billing"));
        
    }
}
