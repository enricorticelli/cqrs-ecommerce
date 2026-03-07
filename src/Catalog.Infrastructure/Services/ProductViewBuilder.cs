using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Marten;

namespace Catalog.Infrastructure.Services;

internal static class ProductViewBuilder
{
    public static async Task<bool> ReferencesExistAsync(
        IQuerySession querySession,
        Guid brandId,
        Guid categoryId,
        IReadOnlyList<Guid> collectionIds,
        CancellationToken cancellationToken)
    {
        var brand = await querySession.LoadAsync<BrandAggregate>(brandId, cancellationToken);
        var category = await querySession.LoadAsync<CategoryAggregate>(categoryId, cancellationToken);
        if (brand is null || category is null || brand.IsDeleted || category.IsDeleted)
        {
            return false;
        }

        if (collectionIds.Count == 0)
        {
            return true;
        }

        var distinctCollectionIds = collectionIds.Distinct().ToArray();
        var existingCollectionIds = await querySession.Query<CollectionAggregate>()
            .Where(x => distinctCollectionIds.Contains(x.Id) && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return distinctCollectionIds.All(existingCollectionIds.Contains);
    }

    public static async Task<IReadOnlyList<ProductView>> BuildProductViewsAsync(
        IQuerySession querySession,
        IReadOnlyList<ProductAggregate> products,
        CancellationToken cancellationToken)
    {
        if (products.Count == 0)
        {
            return [];
        }

        var brandIds = products.Select(p => p.BrandId).Distinct().ToArray();
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToArray();
        var collectionIds = products.SelectMany(p => p.CollectionIds).Distinct().ToArray();

        var brands = await querySession.Query<BrandAggregate>()
            .Where(x => brandIds.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var categories = await querySession.Query<CategoryAggregate>()
            .Where(x => categoryIds.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var collections = await querySession.Query<CollectionAggregate>()
            .Where(x => collectionIds.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var brandMap = brands.ToDictionary(x => x.Id, x => x);
        var categoryMap = categories.ToDictionary(x => x.Id, x => x);
        var collectionMap = collections.ToDictionary(x => x.Id, x => x);

        return products.Select(product => MapToView(product, brandMap, categoryMap, collectionMap)).ToArray();
    }

    public static async Task<ProductView> BuildProductViewAsync(
        IQuerySession querySession,
        ProductAggregate product,
        CancellationToken cancellationToken)
    {
        var views = await BuildProductViewsAsync(querySession, [product], cancellationToken);
        return views[0];
    }

    private static ProductView MapToView(
        ProductAggregate product,
        IReadOnlyDictionary<Guid, BrandAggregate> brandMap,
        IReadOnlyDictionary<Guid, CategoryAggregate> categoryMap,
        IReadOnlyDictionary<Guid, CollectionAggregate> collectionMap)
    {
        var brandName = brandMap.TryGetValue(product.BrandId, out var brand) ? brand.Name : "Unknown brand";
        var categoryName = categoryMap.TryGetValue(product.CategoryId, out var category) ? category.Name : "Unknown category";

        var collectionNames = product.CollectionIds
            .Where(collectionMap.ContainsKey)
            .Select(id => collectionMap[id].Name)
            .ToArray();

        return new ProductView(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.BrandId,
            brandName,
            product.CategoryId,
            categoryName,
            product.CollectionIds,
            collectionNames,
            product.IsNewArrival,
            product.IsBestSeller,
            product.CreatedAtUtc);
    }
}
