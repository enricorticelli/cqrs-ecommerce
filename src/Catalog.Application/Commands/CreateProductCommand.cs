namespace Catalog.Application.Commands;

public sealed record CreateProductCommand(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<Guid> CollectionIds,
    bool IsNewArrival,
    bool IsBestSeller);
