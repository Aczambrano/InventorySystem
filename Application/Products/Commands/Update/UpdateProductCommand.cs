using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Products.Commands.Update;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    string SKU) : IRequest<Result>;
