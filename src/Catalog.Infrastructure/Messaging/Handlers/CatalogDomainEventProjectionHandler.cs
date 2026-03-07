using Catalog.Domain.Events.Brand;
using Catalog.Domain.Events.Category;
using Catalog.Domain.Events.Collection;
using Catalog.Domain.Events.Product;
using Catalog.Infrastructure.Persistence.ReadModels;

namespace Catalog.Infrastructure.Messaging.Handlers;

public sealed class CatalogDomainEventProjectionHandler
{
    public static Task Handle(BrandCreatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
        => store.UpsertBrandAsync(message.BrandId, message.Name, message.Slug, message.Description, false, cancellationToken);

    public static async Task Handle(BrandUpdatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetBrandByIdAsync(message.BrandId, cancellationToken);
        var isDeleted = existing?.IsDeleted ?? false;
        await store.UpsertBrandAsync(message.BrandId, message.Name, message.Slug, message.Description, isDeleted, cancellationToken);
    }

    public static async Task Handle(BrandDeletedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetBrandByIdAsync(message.BrandId, cancellationToken);
        if (existing is null)
        {
            await store.UpsertBrandAsync(message.BrandId, string.Empty, string.Empty, string.Empty, true, cancellationToken);
            return;
        }

        await store.UpsertBrandAsync(existing.Id, existing.Name, existing.Slug, existing.Description, true, cancellationToken);
    }

    public static Task Handle(CategoryCreatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
        => store.UpsertCategoryAsync(message.CategoryId, message.Name, message.Slug, message.Description, false, cancellationToken);

    public static async Task Handle(CategoryUpdatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetCategoryByIdAsync(message.CategoryId, cancellationToken);
        var isDeleted = existing?.IsDeleted ?? false;
        await store.UpsertCategoryAsync(message.CategoryId, message.Name, message.Slug, message.Description, isDeleted, cancellationToken);
    }

    public static async Task Handle(CategoryDeletedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetCategoryByIdAsync(message.CategoryId, cancellationToken);
        if (existing is null)
        {
            await store.UpsertCategoryAsync(message.CategoryId, string.Empty, string.Empty, string.Empty, true, cancellationToken);
            return;
        }

        await store.UpsertCategoryAsync(existing.Id, existing.Name, existing.Slug, existing.Description, true, cancellationToken);
    }

    public static Task Handle(CollectionCreatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
        => store.UpsertCollectionAsync(message.CollectionId, message.Name, message.Slug, message.Description, message.IsFeatured, false, cancellationToken);

    public static async Task Handle(CollectionUpdatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetCollectionByIdAsync(message.CollectionId, cancellationToken);
        var isDeleted = existing?.IsDeleted ?? false;
        await store.UpsertCollectionAsync(message.CollectionId, message.Name, message.Slug, message.Description, message.IsFeatured, isDeleted, cancellationToken);
    }

    public static async Task Handle(CollectionDeletedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetCollectionByIdAsync(message.CollectionId, cancellationToken);
        if (existing is null)
        {
            await store.UpsertCollectionAsync(message.CollectionId, string.Empty, string.Empty, string.Empty, false, true, cancellationToken);
            return;
        }

        await store.UpsertCollectionAsync(existing.Id, existing.Name, existing.Slug, existing.Description, existing.IsFeatured, true, cancellationToken);
    }

    public static Task Handle(ProductCreatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
        => store.UpsertProductAsync(
            message.ProductId,
            message.Sku,
            message.Name,
            message.Description,
            message.Price,
            message.BrandId,
            message.CategoryId,
            message.CollectionIds,
            message.IsNewArrival,
            message.IsBestSeller,
            message.CreatedAtUtc,
            false,
            cancellationToken);

    public static async Task Handle(ProductUpdatedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetProductByIdAsync(message.ProductId, cancellationToken);
        var createdAtUtc = existing?.CreatedAtUtc ?? DateTimeOffset.UtcNow;
        var isDeleted = existing?.IsDeleted ?? false;

        await store.UpsertProductAsync(
            message.ProductId,
            message.Sku,
            message.Name,
            message.Description,
            message.Price,
            message.BrandId,
            message.CategoryId,
            message.CollectionIds,
            message.IsNewArrival,
            message.IsBestSeller,
            createdAtUtc,
            isDeleted,
            cancellationToken);
    }

    public static async Task Handle(ProductDeletedDomainEvent message, CatalogReadModelStore store, CancellationToken cancellationToken)
    {
        var existing = await store.GetProductByIdAsync(message.ProductId, cancellationToken);
        if (existing is null)
        {
            await store.UpsertProductAsync(
                message.ProductId,
                string.Empty,
                string.Empty,
                string.Empty,
                0m,
                Guid.Empty,
                Guid.Empty,
                Array.Empty<Guid>(),
                false,
                false,
                DateTimeOffset.UtcNow,
                true,
                cancellationToken);
            return;
        }

        await store.UpsertProductAsync(
            existing.Id,
            existing.Sku,
            existing.Name,
            existing.Description,
            existing.Price,
            existing.BrandId,
            existing.CategoryId,
            existing.CollectionIds,
            existing.IsNewArrival,
            existing.IsBestSeller,
            existing.CreatedAtUtc,
            true,
            cancellationToken);
    }
}
