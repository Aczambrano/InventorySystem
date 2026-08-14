using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Products.Commands.Stocks;

public record AddProductStockCommand(
    Guid ProductId,
    Guid ProviderId,
    string LotNumber,
    decimal UnitPrice,
    int StockQuantity,
    DateTime? ExpirationDate) : IRequest<Result<Guid>>;

internal sealed class AddProductStockCommandHandler : IRequestHandler<AddProductStockCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public AddProductStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(AddProductStockCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
        {
            return Result.Failure<Guid>(DomainErrors.Product.NotFound);
        }

        var providerExists = await _context.Providers.AnyAsync(p => p.Id == request.ProviderId, cancellationToken);
        if (!providerExists)
        {
            return Result.Failure<Guid>(new Error("Provider.NotFound", "El proveedor especificado no existe."));
        }

        var stock = new ProductStock
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            ProviderId = request.ProviderId,
            LotNumber = request.LotNumber,
            UnitPrice = request.UnitPrice,
            StockQuantity = request.StockQuantity,
            ExpirationDate = request.ExpirationDate
        };

        _context.ProductStocks.Add(stock);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(stock.Id);
    }
}

public record UpdateProductStockCommand(
    Guid StockId,
    string LotNumber,
    decimal UnitPrice,
    int StockQuantity,
    DateTime? ExpirationDate) : IRequest<Result>;

internal sealed class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        var stock = await _context.ProductStocks.SingleOrDefaultAsync(s => s.Id == request.StockId, cancellationToken);
        if (stock is null)
        {
            return Result.Failure(new Error("Stock.NotFound", "Registro de stock no encontrado."));
        }

        stock.LotNumber = request.LotNumber;
        stock.UnitPrice = request.UnitPrice;
        stock.StockQuantity = request.StockQuantity;
        stock.ExpirationDate = request.ExpirationDate;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public record DeleteProductStockCommand(Guid StockId) : IRequest<Result>;

internal sealed class DeleteProductStockCommandHandler : IRequestHandler<DeleteProductStockCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteProductStockCommand request, CancellationToken cancellationToken)
    {
        var stock = await _context.ProductStocks.SingleOrDefaultAsync(s => s.Id == request.StockId, cancellationToken);
        if (stock is null)
        {
            return Result.Failure(new Error("Stock.NotFound", "Registro de stock no encontrado."));
        }

        _context.ProductStocks.Remove(stock);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
