namespace InventorySystem.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
}
