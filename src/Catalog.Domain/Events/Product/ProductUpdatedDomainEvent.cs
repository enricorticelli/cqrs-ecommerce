namespace Catalog.Domain.Events.Product;

public sealed record ProductUpdatedDomainEvent(
    Guid ProductId,
    string Sku,
    string Name,
    string Description,
    decimal Price,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<Guid> CollectionIds,
    bool IsNewArrival,
    bool IsBestSeller);
