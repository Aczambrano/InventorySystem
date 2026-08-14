using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Products.Commands.Create;

internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var providerExists = await _context.Providers.AnyAsync(p => p.Id == request.ProviderId, cancellationToken);
        if (!providerExists)
        {
            return Result.Failure<Guid>(new Error("Provider.NotFound", "The specified provider was not found."));
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            SKU = request.SKU
        };

        var stock = new ProductStock
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            ProviderId = request.ProviderId,
            LotNumber = request.LotNumber,
            UnitPrice = request.UnitPrice,
            StockQuantity = request.StockQuantity,
            ExpirationDate = request.ExpirationDate
        };

        product.Stocks.Add(stock);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}
