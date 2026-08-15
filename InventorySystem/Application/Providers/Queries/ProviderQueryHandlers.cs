using InventorySystem.Application.Data;
using InventorySystem.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Providers.Queries;

internal sealed class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, Result<List<ProviderResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetProvidersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ProviderResponse>>> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await _context.Providers
            .AsNoTracking()
            .Select(p => new ProviderResponse(p.Id, p.Name, p.TaxId, p.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Success(providers);
    }
}

internal sealed class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery, Result<ProviderResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetProviderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProviderResponse>> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
    {
        var provider = await _context.Providers
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProviderResponse(p.Id, p.Name, p.TaxId, p.IsActive))
            .SingleOrDefaultAsync(cancellationToken);

        if (provider is null)
        {
            return Result.Failure<ProviderResponse>(new Error("Provider.NotFound", "Proveedor no encontrado."));
        }

        return Result.Success(provider);
    }
}
