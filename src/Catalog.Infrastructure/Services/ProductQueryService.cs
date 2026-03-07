using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Infrastructure.Persistence.ReadModels;

namespace Catalog.Infrastructure.Services;

public sealed class ProductQueryService(CatalogReadModelStore readModelStore) : IProductQueryService
{
    public async Task<IReadOnlyList<ProductView>> GetProductsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListProductsAsync(limit, offset, searchTerm, cancellationToken);
        return await ToViewsAsync(rows, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListNewArrivalsAsync(cancellationToken);
        return await ToViewsAsync(rows, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetBestSellersAsync(CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListBestSellersAsync(cancellationToken);
        return await ToViewsAsync(rows, cancellationToken);
    }

    public async Task<ProductView?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await readModelStore.GetProductByIdAsync(id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        var list = await ToViewsAsync([row], cancellationToken);
        return list.FirstOrDefault();
    }

    private async Task<IReadOnlyList<ProductView>> ToViewsAsync(IReadOnlyList<ProductReadModelRow> rows, CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var brands = await readModelStore.GetBrandsByIdsAsync(rows.Select(x => x.BrandId), cancellationToken);
        var categories = await readModelStore.GetCategoriesByIdsAsync(rows.Select(x => x.CategoryId), cancellationToken);
        var collections = await readModelStore.GetCollectionsByIdsAsync(rows.SelectMany(x => x.CollectionIds), cancellationToken);

        return rows.Select(x => new ProductView(
            x.Id,
            x.Sku,
            x.Name,
            x.Description,
            x.Price,
            x.BrandId,
            brands.TryGetValue(x.BrandId, out var brand) ? brand.Name : "Unknown brand",
            x.CategoryId,
            categories.TryGetValue(x.CategoryId, out var category) ? category.Name : "Unknown category",
            x.CollectionIds,
            x.CollectionIds.Where(collections.ContainsKey).Select(id => collections[id].Name).ToArray(),
            x.IsNewArrival,
            x.IsBestSeller,
            x.CreatedAtUtc)).ToArray();
    }
}
