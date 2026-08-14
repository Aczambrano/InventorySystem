using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Providers.Commands.Delete;

public record DeleteProviderCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteProviderCommandHandler : IRequestHandler<DeleteProviderCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.Providers.SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (provider is null)
        {
            return Result.Failure(new Error("Provider.NotFound", "Proveedor no encontrado."));
        }

        var hasStocks = await _context.ProductStocks.AnyAsync(ps => ps.ProviderId == request.Id, cancellationToken);
        if (hasStocks)
        {
            return Result.Failure(new Error("Provider.HasDependencies", "No se puede eliminar el proveedor porque tiene productos/lotes asociados en el inventario."));
        }

        _context.Providers.Remove(provider);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
