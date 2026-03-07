using Catalog.Domain.Events;
using Catalog.Domain.Events.Product;

namespace Catalog.Domain.Aggregates;

public sealed class ProductAggregate
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public Guid BrandId { get; private set; }
    public Guid CategoryId { get; private set; }
    public IReadOnlyList<Guid> CollectionIds { get; private set; } = [];
    public bool IsNewArrival { get; private set; }
    public bool IsBestSeller { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Apply(ProductCreatedDomainEvent @event)
    {
        Id = @event.ProductId;
        Sku = @event.Sku;
        Name = @event.Name;
        Description = @event.Description;
        Price = @event.Price;
        BrandId = @event.BrandId;
        CategoryId = @event.CategoryId;
        CollectionIds = @event.CollectionIds.Distinct().ToArray();
        IsNewArrival = @event.IsNewArrival;
        IsBestSeller = @event.IsBestSeller;
        CreatedAtUtc = @event.CreatedAtUtc;
        IsDeleted = false;
    }

    public void Apply(ProductUpdatedDomainEvent @event)
    {
        Sku = @event.Sku;
        Name = @event.Name;
        Description = @event.Description;
        Price = @event.Price;
        BrandId = @event.BrandId;
        CategoryId = @event.CategoryId;
        CollectionIds = @event.CollectionIds.Distinct().ToArray();
        IsNewArrival = @event.IsNewArrival;
        IsBestSeller = @event.IsBestSeller;
    }

    public void Apply(ProductDeletedDomainEvent _)
    {
        IsDeleted = true;
    }
}
