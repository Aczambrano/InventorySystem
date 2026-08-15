namespace InventorySystem.Domain.Entities;

public class ProductStock
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }

    public string LotNumber { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
