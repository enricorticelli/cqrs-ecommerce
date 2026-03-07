using Catalog.Application.Views;

namespace Catalog.Application.Abstractions.Products;

public interface IProductService
{
    Task<IReadOnlyList<ProductView>> GetProductsAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductView>> GetBestSellersAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<ProductView> GetProductAsync(Guid id, CancellationToken cancellationToken);

    Task<ProductView> CreateProductAsync(string sku, string name, string description, decimal price, Guid brandId,
        Guid categoryId, IReadOnlyList<Guid> collectionIds, bool isNewArrival, bool isBestSeller, string correlationId,
        CancellationToken cancellationToken);

    Task<ProductView> UpdateProductAsync(Guid id, string sku, string name, string description, decimal price,
        Guid brandId, Guid categoryId, IReadOnlyList<Guid> collectionIds, bool isNewArrival, bool isBestSeller,
        string correlationId, CancellationToken cancellationToken);

    Task DeleteProductAsync(Guid id, string correlationId, CancellationToken cancellationToken);
}
