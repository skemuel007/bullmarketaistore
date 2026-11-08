using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BullMarket.Infrastructure.Persistence.Configurations;

internal sealed class ProductTagConfiguration : IEntityTypeConfiguration<ProductTags>
{
    public void Configure(EntityTypeBuilder<ProductTags> builder)
    {
        builder.ToTable("product_tags");
        builder.HasKey(tag => tag.Id);
        builder.Property(tag => tag.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(tag => new { tag.ProductId, tag.Name }).IsUnique();
    }
}
