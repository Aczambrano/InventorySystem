using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Infrastructure.Persistence.Configurations;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.TaxId)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.HasIndex(p => p.TaxId)
            .IsUnique();

        builder.HasMany(p => p.ProductStocks)
            .WithOne(ps => ps.Provider)
            .HasForeignKey(ps => ps.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
