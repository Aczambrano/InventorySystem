using InventorySystem.Application.Data;
using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InventorySystem.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
