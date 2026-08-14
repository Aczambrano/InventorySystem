using InventorySystem.Application.Data;
using InventorySystem.Application.Products.Queries.GetAll;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Products.Queries.GetById;

internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Stocks)
            .ThenInclude(s => s.Provider)
            .Where(p => p.Id == request.Id)
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
            .SingleOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(DomainErrors.Product.NotFound);
        }

        return Result.Success(product);
    }
}
