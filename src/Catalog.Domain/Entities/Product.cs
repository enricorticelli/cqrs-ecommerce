namespace Catalog.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid BrandId { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsBestSeller { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }

    public Brand Brand { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public List<ProductCollection> ProductCollections { get; set; } = [];
}
