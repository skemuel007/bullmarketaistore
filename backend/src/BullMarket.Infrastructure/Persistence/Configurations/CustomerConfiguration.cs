using BullMarket.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BullMarket.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(customer => customer.LastName).HasMaxLength(100).IsRequired();
        builder.Property(customer => customer.Email).HasMaxLength(320).IsRequired();
        builder.Property(customer => customer.Phone).HasMaxLength(30);
        builder.HasIndex(customer => customer.Email).IsUnique();
        
        builder.OwnsOne(customer => customer.ShippingAddress, address => address.MapAddress("shipping"));
        builder.OwnsOne(customer => customer.BillingAddress, address => address.MapAddress("billing"));
    }
}
