using InventorySystem.Domain.Common;
using MediatR;

namespace InventorySystem.Application.Products.Queries.GetAll;

public record GetProductsQuery() : IRequest<Result<List<ProductResponse>>>;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    string SKU,
    List<ProductStockResponse> Stocks);

public record ProductStockResponse(
    Guid Id,
    string ProviderName,
    string LotNumber,
    decimal UnitPrice,
    int StockQuantity,
    DateTime? ExpirationDate);
