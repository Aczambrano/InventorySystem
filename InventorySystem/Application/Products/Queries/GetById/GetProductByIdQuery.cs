using InventorySystem.Application.Products.Queries.GetAll;
using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Products.Queries.GetById;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductResponse>>;
