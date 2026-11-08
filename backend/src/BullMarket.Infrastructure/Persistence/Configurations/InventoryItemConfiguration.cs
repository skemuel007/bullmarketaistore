using BullMarket.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BullMarket.Infrastructure.Persistence.Configurations;

internal sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");
        builder.HasKey(item => item.Id);
        builder.Ignore(item => item.QuantityAvailable);
        builder.HasIndex(item => item.ProductId).IsUnique();
        builder.HasOne<Product>()
            .WithOne()
            .HasForeignKey<InventoryItem>(item => item.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        // Version concurrency handling is configured in the DbContext (xmin on PostgreSQL).
    }
}
