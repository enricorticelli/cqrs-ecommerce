using Catalog.Application;
using Catalog.Application.Abstractions;
using Catalog.Application.Brands;
using Catalog.Application.Categories;
using Catalog.Application.Collections;
using Catalog.Application.Products;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Marten;

namespace Catalog.Infrastructure;

public class CatalogService(IQuerySession querySession, IDocumentSession documentSession) : ICatalogService
{
    public async Task<IReadOnlyList<BrandView>> GetBrandsAsync(int limit, int offset,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var brands = await querySession.Query<BrandAggregate>()
            .OrderBy<BrandAggregate, string>(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return brands.Select(MapToView).ToArray();
    }

    public async Task<BrandView?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var brand = await querySession.LoadAsync<BrandAggregate>(id, cancellationToken);
        return brand is null ? null : MapToView(brand);
    }

    public async Task<BrandView> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = new BrandAggregate
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        documentSession.Store(brand);
        await documentSession.SaveChangesAsync(cancellationToken);
        return MapToView(brand);
    }

    public async Task<BrandView?> UpdateBrandAsync(Guid id, UpdateBrandCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<BrandAggregate>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new BrandAggregate
        {
            Id = id,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        documentSession.Store(updated);
        await documentSession.SaveChangesAsync(cancellationToken);
        return MapToView(updated);
    }

    public async Task<bool> DeleteBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var brand = await documentSession.LoadAsync<BrandAggregate>(id, cancellationToken);
        if (brand is null)
        {
            return false;
        }

        documentSession.Delete<BrandAggregate>(id);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static BrandView MapToView(BrandAggregate brand)
    {
        return new BrandView(brand.Id, brand.Name, brand.Slug, brand.Description);
    }

    public async Task<IReadOnlyList<CategoryView>> GetCategoriesAsync(int limit, int offset,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var categories = await querySession.Query<CategoryAggregate>()
            .OrderBy<CategoryAggregate, string>(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return categories.Select(MapToView).ToArray();
    }

    public async Task<CategoryView?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await querySession.LoadAsync<CategoryAggregate>(id, cancellationToken);
        return category is null ? null : MapToView(category);
    }

    public async Task<CategoryView> CreateCategoryAsync(CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = new CategoryAggregate
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        documentSession.Store(category);
        await documentSession.SaveChangesAsync(cancellationToken);
        return MapToView(category);
    }

    public async Task<CategoryView?> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<CategoryAggregate>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new CategoryAggregate
        {
            Id = id,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        documentSession.Store(updated);
        await documentSession.SaveChangesAsync(cancellationToken);
        return MapToView(updated);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await documentSession.LoadAsync<CategoryAggregate>(id, cancellationToken);
        if (category is null)
        {
            return false;
        }

        documentSession.Delete<CategoryAggregate>(id);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CategoryView MapToView(CategoryAggregate category)
    {
        return new CategoryView(category.Id, category.Name, category.Slug, category.Description);
    }

    public async Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(int limit, int offset,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var collections = await querySession.Query<CollectionAggregate>()
            .OrderBy<CollectionAggregate, string>(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return collections.Select(MapToView).ToArray();
    }

    public async Task<CollectionView?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await querySession.LoadAsync<CollectionAggregate>(id, cancellationToken);
        return collection is null ? null : MapToView(collection);
    }

    public async Task<CollectionView> CreateCollectionAsync(CreateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var collection = new CollectionAggregate
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            IsFeatured = command.IsFeatured
        };

        documentSession.Store(collection);
        await documentSession.SaveChangesAsync(cancellationToken);
        return MapToView(collection);
    }

    public async Task<CollectionView?> UpdateCollectionAsync(Guid id, UpdateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<CollectionAggregate>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new CollectionAggregate
        {
            Id = id,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            IsFeatured = command.IsFeatured
        };

        documentSession.Store(updated);
        await documentSession.SaveChangesAsync(cancellationToken);
        return MapToView(updated);
    }

    public async Task<bool> DeleteCollectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await documentSession.LoadAsync<CollectionAggregate>(id, cancellationToken);
        if (collection is null)
        {
            return false;
        }

        documentSession.Delete<CollectionAggregate>(id);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CollectionView MapToView(CollectionAggregate collection)
    {
        return new CollectionView(collection.Id, collection.Name, collection.Slug, collection.Description,
            collection.IsFeatured);
    }

    private async Task<bool> ReferencesExistAsync(Guid brandId, Guid categoryId, IReadOnlyList<Guid> collectionIds,
        CancellationToken cancellationToken)
    {
        var brand = await querySession.LoadAsync<BrandAggregate>(brandId, cancellationToken);
        var category = await querySession.LoadAsync<CategoryAggregate>(categoryId, cancellationToken);
        if (brand is null || category is null)
        {
            return false;
        }

        if (collectionIds.Count == 0)
        {
            return true;
        }

        var existingCollectionIds = await querySession.Query<CollectionAggregate>()
            .Where<CollectionAggregate>(x => collectionIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return collectionIds.Distinct().All(existingCollectionIds.Contains);
    }

    private async Task<IReadOnlyList<ProductView>> BuildProductViewsAsync(IReadOnlyList<ProductAggregate> products,
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
            .Where<BrandAggregate>(x => brandIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var categories = await querySession.Query<CategoryAggregate>()
            .Where<CategoryAggregate>(x => categoryIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var collections = await querySession.Query<CollectionAggregate>()
            .Where<CollectionAggregate>(x => collectionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var brandMap = brands.ToDictionary(x => x.Id, x => x);
        var categoryMap = categories.ToDictionary(x => x.Id, x => x);
        var collectionMap = collections.ToDictionary(x => x.Id, x => x);

        return products.Select(p => MapToView(p, brandMap, categoryMap, collectionMap)).ToArray();
    }

    private async Task<ProductView> BuildProductViewAsync(ProductAggregate product, CancellationToken cancellationToken)
    {
        var brands = await querySession.Query<BrandAggregate>()
            .Where<BrandAggregate>(x => x.Id == product.BrandId)
            .ToListAsync(cancellationToken);

        var categories = await querySession.Query<CategoryAggregate>()
            .Where<CategoryAggregate>(x => x.Id == product.CategoryId)
            .ToListAsync(cancellationToken);

        var collections = await querySession.Query<CollectionAggregate>()
            .Where<CollectionAggregate>(x => product.CollectionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return MapToView(
            product,
            brands.ToDictionary(x => x.Id, x => x),
            categories.ToDictionary(x => x.Id, x => x),
            collections.ToDictionary(x => x.Id, x => x));
    }

    private static ProductView MapToView(
        ProductAggregate product,
        IReadOnlyDictionary<Guid, BrandAggregate> brandMap,
        IReadOnlyDictionary<Guid, CategoryAggregate> categoryMap,
        IReadOnlyDictionary<Guid, CollectionAggregate> collectionMap)
    {
        var brandName = brandMap.TryGetValue(product.BrandId, out var brand) ? brand.Name : "Unknown brand";
        var categoryName = categoryMap.TryGetValue(product.CategoryId, out var category)
            ? category.Name
            : "Unknown category";

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

    public async Task<IReadOnlyList<ProductView>> GetProductsAsync(int limit, int offset,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var products = await querySession.Query<ProductAggregate>()
            .OrderBy<ProductAggregate, string>(p => p.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return await BuildProductViewsAsync(products, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(CancellationToken cancellationToken)
    {
        var products = await querySession.Query<ProductAggregate>()
            .Where<ProductAggregate>(p => p.IsNewArrival)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return await BuildProductViewsAsync(products, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetBestSellersAsync(CancellationToken cancellationToken)
    {
        var products = await querySession.Query<ProductAggregate>()
            .Where<ProductAggregate>(p => p.IsBestSeller)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return await BuildProductViewsAsync(products, cancellationToken);
    }

    public async Task<ProductView?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await querySession.LoadAsync<ProductAggregate>(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        return await BuildProductViewAsync(product, cancellationToken);
    }

    public async Task<ProductView?> CreateProductAsync(CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var isValidReference = await ReferencesExistAsync(command.BrandId, command.CategoryId, command.CollectionIds,
            cancellationToken);
        if (!isValidReference)
        {
            return null;
        }

        var product = new ProductAggregate
        {
            Id = Guid.NewGuid(),
            Sku = command.Sku,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            BrandId = command.BrandId,
            CategoryId = command.CategoryId,
            CollectionIds = command.CollectionIds.Distinct().ToArray(),
            IsNewArrival = command.IsNewArrival,
            IsBestSeller = command.IsBestSeller,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        documentSession.Store(product);
        await documentSession.SaveChangesAsync(cancellationToken);

        return await BuildProductViewAsync(product, cancellationToken);
    }

    public async Task<ProductView?> UpdateProductAsync(Guid id, UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<ProductAggregate>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var isValidReference = await ReferencesExistAsync(command.BrandId, command.CategoryId, command.CollectionIds,
            cancellationToken);
        if (!isValidReference)
        {
            return null;
        }

        var updated = new ProductAggregate
        {
            Id = id,
            Sku = command.Sku,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            BrandId = command.BrandId,
            CategoryId = command.CategoryId,
            CollectionIds = command.CollectionIds.Distinct().ToArray(),
            IsNewArrival = command.IsNewArrival,
            IsBestSeller = command.IsBestSeller,
            CreatedAtUtc = existing.CreatedAtUtc
        };

        documentSession.Store(updated);
        await documentSession.SaveChangesAsync(cancellationToken);

        return await BuildProductViewAsync(updated, cancellationToken);
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var existing = await documentSession.LoadAsync<ProductAggregate>(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        documentSession.Delete<ProductAggregate>(id);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}