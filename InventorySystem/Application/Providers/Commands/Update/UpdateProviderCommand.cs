using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Providers.Commands.Update;

public record UpdateProviderCommand(Guid Id, string Name, string TaxId, bool IsActive) : IRequest<Result>;

internal sealed class UpdateProviderCommandHandler : IRequestHandler<UpdateProviderCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.Providers.SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (provider is null)
        {
            return Result.Failure(new Error("Provider.NotFound", "Proveedor no encontrado."));
        }

        var duplicateTaxId = await _context.Providers
            .AnyAsync(p => p.TaxId == request.TaxId && p.Id != request.Id, cancellationToken);

        if (duplicateTaxId)
        {
            return Result.Failure(new Error("Provider.DuplicateTaxId", "Ya existe otro proveedor con ese RUC / TaxId."));
        }

        provider.Name = request.Name;
        provider.TaxId = request.TaxId;
        provider.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
