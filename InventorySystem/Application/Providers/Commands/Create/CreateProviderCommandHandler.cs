using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Providers.Commands.Create;

internal sealed class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Providers.AnyAsync(p => p.TaxId == request.TaxId, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("Provider.DuplicateTaxId", "Ya existe un proveedor con ese RUC / TaxId."));
        }

        var provider = new Provider
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TaxId = request.TaxId,
            IsActive = true
        };

        _context.Providers.Add(provider);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(provider.Id);
    }
}
