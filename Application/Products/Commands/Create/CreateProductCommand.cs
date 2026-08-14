using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Products.Commands.Create;

public record CreateProductCommand(
    string Name,
    string Description,
    string SKU,
    Guid ProviderId,
    string LotNumber,
    decimal UnitPrice,
    int StockQuantity,
    DateTime? ExpirationDate) : IRequest<Result<Guid>>;
