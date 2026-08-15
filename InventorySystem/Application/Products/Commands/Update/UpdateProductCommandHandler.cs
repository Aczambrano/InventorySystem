using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Products.Commands.Update;

internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound);
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.SKU = request.SKU;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
