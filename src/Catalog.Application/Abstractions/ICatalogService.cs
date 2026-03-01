using Catalog.Domain;

namespace Catalog.Application;

public interface ICatalogService
{
    Task<IReadOnlyList<ProductView>> GetProductsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductView>> GetBestSellersAsync(CancellationToken cancellationToken);
    Task<ProductView?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductView?> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);
    Task<ProductView?> UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<BrandDocument>> GetBrandsAsync(CancellationToken cancellationToken);
    Task<BrandDocument?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BrandDocument> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken);
    Task<BrandDocument?> UpdateBrandAsync(Guid id, UpdateBrandCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteBrandAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CategoryDocument>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<CategoryDocument?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CategoryDocument> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken);
    Task<CategoryDocument?> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CollectionDocument>> GetCollectionsAsync(CancellationToken cancellationToken);
    Task<CollectionDocument?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CollectionDocument> CreateCollectionAsync(CreateCollectionCommand command, CancellationToken cancellationToken);
    Task<CollectionDocument?> UpdateCollectionAsync(Guid id, UpdateCollectionCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteCollectionAsync(Guid id, CancellationToken cancellationToken);
}
