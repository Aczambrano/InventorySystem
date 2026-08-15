using InventorySystem.Application.Products.Commands.Create;
using InventorySystem.Application.Products.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetProductsQuery();
        var result = await _sender.Send(query);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new InventorySystem.Application.Products.Queries.GetById.GetProductByIdQuery(id);
        var result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InventorySystem.Application.Products.Commands.Update.UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { error = "El ID de la ruta no coincide con el del cuerpo de la petición." });
        }

        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new InventorySystem.Application.Products.Commands.Delete.DeleteProductCommand(id);
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPost("{id}/stocks")]
    public async Task<IActionResult> AddStock(Guid id, [FromBody] AddStockRequest request)
    {
        var command = new InventorySystem.Application.Products.Commands.Stocks.AddProductStockCommand(
            id,
            request.ProviderId,
            request.LotNumber,
            request.UnitPrice,
            request.StockQuantity,
            request.ExpirationDate);

        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { StockId = result.Value });
    }

    [HttpPut("stocks/{stockId}")]
    public async Task<IActionResult> UpdateStock(Guid stockId, [FromBody] UpdateStockRequest request)
    {
        var command = new InventorySystem.Application.Products.Commands.Stocks.UpdateProductStockCommand(
            stockId,
            request.LotNumber,
            request.UnitPrice,
            request.StockQuantity,
            request.ExpirationDate);

        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("stocks/{stockId}")]
    public async Task<IActionResult> DeleteStock(Guid stockId)
    {
        var command = new InventorySystem.Application.Products.Commands.Stocks.DeleteProductStockCommand(stockId);
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}

public record AddStockRequest(Guid ProviderId, string LotNumber, decimal UnitPrice, int StockQuantity, DateTime? ExpirationDate);
public record UpdateStockRequest(string LotNumber, decimal UnitPrice, int StockQuantity, DateTime? ExpirationDate);

