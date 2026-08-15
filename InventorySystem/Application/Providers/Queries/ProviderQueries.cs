using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Providers.Queries;

public record ProviderResponse(Guid Id, string Name, string TaxId, bool IsActive);

public record GetProvidersQuery() : IRequest<Result<List<ProviderResponse>>>;

public record GetProviderByIdQuery(Guid Id) : IRequest<Result<ProviderResponse>>;
