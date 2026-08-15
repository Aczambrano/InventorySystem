using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Providers.Commands.Create;

public record CreateProviderCommand(string Name, string TaxId) : IRequest<Result<Guid>>;
