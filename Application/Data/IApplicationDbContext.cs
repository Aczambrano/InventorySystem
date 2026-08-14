using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
    DbSet<Provider> Providers { get; }
    DbSet<ProductStock> ProductStocks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
