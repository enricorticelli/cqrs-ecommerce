namespace Catalog.Domain.Entities;

public sealed class ProductCollection
{
    public Guid ProductId { get; set; }
    public Guid CollectionId { get; set; }

    public Product Product { get; set; } = null!;
    public CatalogCollection Collection { get; set; } = null!;
}
