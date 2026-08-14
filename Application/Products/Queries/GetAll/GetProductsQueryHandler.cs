using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Products.Queries.GetAll;

internal sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<List<ProductResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Stocks)
            .ThenInclude(s => s.Provider)
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.SKU,
                p.Stocks.Select(s => new ProductStockResponse(
                    s.Id,
                    s.Provider!.Name,
                    s.LotNumber,
                    s.UnitPrice,
                    s.StockQuantity,
                    s.ExpirationDate
                )).ToList()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(products);
    }
}
