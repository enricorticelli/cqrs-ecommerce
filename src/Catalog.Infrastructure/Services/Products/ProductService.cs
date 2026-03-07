using Catalog.Application.Abstractions;
using Catalog.Application.Abstractions.Products;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services.Common;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Catalog;
using Shared.BuildingBlocks.Exceptions;
using Wolverine.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services.Products;

public sealed class ProductService(
    CatalogDbContext dbContext,
    IDbContextOutbox<CatalogDbContext> outbox) : CatalogServiceBase(dbContext, outbox), IProductService, IProductCommandService
{
    public async Task<IReadOnlyList<ProductView>> GetProductsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        return await QueryProductsAsync(DbContext.Products.AsQueryable(), searchTerm, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        return await QueryProductsAsync(DbContext.Products.Where(x => x.IsNewArrival), searchTerm, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductView>> GetBestSellersAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        return await QueryProductsAsync(DbContext.Products.Where(x => x.IsBestSeller), searchTerm, cancellationToken);
    }

    public async Task<ProductView> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var products = await QueryProductsAsync(DbContext.Products.Where(x => x.Id == id), null, cancellationToken);
        return products.FirstOrDefault() ?? throw new NotFoundAppException($"Product '{id}' not found.");
    }

    public async Task<ProductView> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return await CreateProductAsync(
            command.Sku,
            command.Name,
            command.Description,
            command.Price,
            command.BrandId,
            command.CategoryId,
            command.CollectionIds,
            command.IsNewArrival,
            command.IsBestSeller,
            Guid.NewGuid().ToString("N"),
            cancellationToken);
    }

    public async Task<ProductView> CreateProductAsync(string sku, string name, string description, decimal price, Guid brandId, Guid categoryId, IReadOnlyList<Guid> collectionIds, bool isNewArrival, bool isBestSeller, string correlationId, CancellationToken cancellationToken)
    {
        CatalogValidationRules.EnsurePriceIsPositive(price);
        await EnsureSkuUniqueAsync(sku, null, cancellationToken);
        await EnsureReferencesExistAsync(brandId, categoryId, collectionIds, cancellationToken);

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Sku = sku.Trim(),
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            BrandId = brandId,
            CategoryId = categoryId,
            IsNewArrival = isNewArrival,
            IsBestSeller = isBestSeller,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var normalizedCollectionIds = DistinctIds(collectionIds);
        foreach (var collectionId in normalizedCollectionIds)
        {
            entity.ProductCollections.Add(new ProductCollection { ProductId = entity.Id, CollectionId = collectionId });
        }

        DbContext.Products.Add(entity);
        await PublishAndFlushAsync(new ProductCreatedV1(entity.Id, entity.Sku, entity.BrandId, entity.CategoryId, normalizedCollectionIds, CreateMetadata(correlationId)), cancellationToken);

        return await GetProductAsync(entity.Id, cancellationToken);
    }

    public async Task<ProductView> UpdateProductAsync(Guid id, string sku, string name, string description, decimal price, Guid brandId, Guid categoryId, IReadOnlyList<Guid> collectionIds, bool isNewArrival, bool isBestSeller, string correlationId, CancellationToken cancellationToken)
    {
        CatalogValidationRules.EnsurePriceIsPositive(price);

        var entity = await DbContext.Products.Include(x => x.ProductCollections).FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Product '{id}' not found.");

        await EnsureSkuUniqueAsync(sku, id, cancellationToken);
        await EnsureReferencesExistAsync(brandId, categoryId, collectionIds, cancellationToken);

        entity.Sku = sku.Trim();
        entity.Name = name.Trim();
        entity.Description = description.Trim();
        entity.Price = price;
        entity.BrandId = brandId;
        entity.CategoryId = categoryId;
        entity.IsNewArrival = isNewArrival;
        entity.IsBestSeller = isBestSeller;

        DbContext.ProductCollections.RemoveRange(entity.ProductCollections);
        var normalizedCollectionIds = DistinctIds(collectionIds);
        foreach (var collectionId in normalizedCollectionIds)
        {
            DbContext.ProductCollections.Add(new ProductCollection { ProductId = entity.Id, CollectionId = collectionId });
        }

        await PublishAndFlushAsync(new ProductUpdatedV1(entity.Id, entity.Sku, entity.BrandId, entity.CategoryId, normalizedCollectionIds, CreateMetadata(correlationId)), cancellationToken);

        return await GetProductAsync(id, cancellationToken);
    }

    public async Task DeleteProductAsync(Guid id, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Products.Include(x => x.ProductCollections).FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Product '{id}' not found.");

        DbContext.ProductCollections.RemoveRange(entity.ProductCollections);
        DbContext.Products.Remove(entity);

        await PublishAndFlushAsync(new ProductDeletedV1(id, CreateMetadata(correlationId)), cancellationToken);
    }

    private async Task<IReadOnlyList<ProductView>> QueryProductsAsync(IQueryable<Product> queryable, string? searchTerm, CancellationToken cancellationToken)
    {
        var normalizedSearch = searchTerm?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            queryable = queryable.Where(x =>
                x.Sku.ToLower().Contains(normalizedSearch) ||
                x.Name.ToLower().Contains(normalizedSearch) ||
                x.Description.ToLower().Contains(normalizedSearch) ||
                x.Brand.Name.ToLower().Contains(normalizedSearch) ||
                x.Category.Name.ToLower().Contains(normalizedSearch) ||
                x.ProductCollections.Any(pc => pc.Collection.Name.ToLower().Contains(normalizedSearch)));
        }

        var entities = await queryable
            .Include(x => x.Brand)
            .Include(x => x.Category)
            .Include(x => x.ProductCollections)
            .ThenInclude(x => x.Collection)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToView).ToList();
    }

    private static ProductView MapToView(Product entity)
    {
        var collectionIds = entity.ProductCollections.Select(x => x.CollectionId).ToArray();
        var collectionNames = entity.ProductCollections.Select(x => x.Collection.Name).ToArray();

        return new ProductView(
            entity.Id,
            entity.Sku,
            entity.Name,
            entity.Description,
            entity.Price,
            entity.BrandId,
            entity.Brand.Name,
            entity.CategoryId,
            entity.Category.Name,
            collectionIds,
            collectionNames,
            entity.IsNewArrival,
            entity.IsBestSeller,
            entity.CreatedAtUtc);
    }

    private async Task EnsureSkuUniqueAsync(string sku, Guid? currentId, CancellationToken cancellationToken)
    {
        var exists = await DbContext.Products.AnyAsync(x => x.Sku == sku && (!currentId.HasValue || x.Id != currentId.Value), cancellationToken);
        if (exists)
        {
            throw new ConflictAppException($"Product SKU '{sku}' already exists.");
        }
    }

    private async Task EnsureReferencesExistAsync(Guid brandId, Guid categoryId, IReadOnlyList<Guid> collectionIds, CancellationToken cancellationToken)
    {
        var hasBrand = await DbContext.Brands.AnyAsync(x => x.Id == brandId, cancellationToken);
        if (!hasBrand)
        {
            throw new ValidationAppException($"Brand '{brandId}' does not exist.");
        }

        var hasCategory = await DbContext.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken);
        if (!hasCategory)
        {
            throw new ValidationAppException($"Category '{categoryId}' does not exist.");
        }

        var normalizedCollectionIds = DistinctIds(collectionIds);
        var collectionCount = await DbContext.Collections.CountAsync(x => normalizedCollectionIds.Contains(x.Id), cancellationToken);
        if (collectionCount != normalizedCollectionIds.Count)
        {
            throw new ValidationAppException("One or more collections do not exist.");
        }
    }
}
