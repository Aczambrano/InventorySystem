namespace InventorySystem.Domain.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty; // RUC
    public bool IsActive { get; set; } = true;

    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
}
