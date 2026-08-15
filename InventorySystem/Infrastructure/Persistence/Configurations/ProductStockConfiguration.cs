using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.LotNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ps => ps.UnitPrice)
            .HasColumnType("decimal(18,2)");

    }
}
