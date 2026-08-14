using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Products.Commands.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;
