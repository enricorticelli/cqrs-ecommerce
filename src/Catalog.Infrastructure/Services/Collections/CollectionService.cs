using Catalog.Application.Abstractions.Collections;
using Catalog.Application.Views;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services.Common;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Catalog;
using Shared.BuildingBlocks.Exceptions;
using Wolverine.EntityFrameworkCore;

namespace Catalog.Infrastructure.Services.Collections;

public sealed class CollectionService(
    CatalogDbContext dbContext,
    IDbContextOutbox<CatalogDbContext> outbox) : CatalogServiceBase(dbContext, outbox), ICollectionService
{
    public async Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var normalizedSearch = searchTerm?.Trim().ToLowerInvariant();
        var query = DbContext.Collections.AsQueryable();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(normalizedSearch) ||
                x.Slug.ToLower().Contains(normalizedSearch) ||
                x.Description.ToLower().Contains(normalizedSearch));
        }

        return await query.OrderBy(x => x.Name)
            .Select(x => new CollectionView(x.Id, x.Name, x.Slug, x.Description, x.IsFeatured))
            .ToListAsync(cancellationToken);
    }

    public async Task<CollectionView> GetCollectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Collections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Collection '{id}' not found.");

        return new CollectionView(entity.Id, entity.Name, entity.Slug, entity.Description, entity.IsFeatured);
    }

    public async Task<CollectionView> CreateCollectionAsync(string name, string slug, string description, bool isFeatured, string correlationId, CancellationToken cancellationToken)
    {
        await EnsureSlugUniqueAsync(slug, null, cancellationToken);

        var entity = new CatalogCollection
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = slug.Trim(),
            Description = description.Trim(),
            IsFeatured = isFeatured
        };

        DbContext.Collections.Add(entity);
        await PublishAndFlushAsync(new CollectionCreatedV1(entity.Id, entity.Name, entity.Slug, entity.IsFeatured, CreateMetadata(correlationId)), cancellationToken);

        return new CollectionView(entity.Id, entity.Name, entity.Slug, entity.Description, entity.IsFeatured);
    }

    public async Task<CollectionView> UpdateCollectionAsync(Guid id, string name, string slug, string description, bool isFeatured, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Collections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Collection '{id}' not found.");

        await EnsureSlugUniqueAsync(slug, id, cancellationToken);

        entity.Name = name.Trim();
        entity.Slug = slug.Trim();
        entity.Description = description.Trim();
        entity.IsFeatured = isFeatured;

        await PublishAndFlushAsync(new CollectionUpdatedV1(entity.Id, entity.Name, entity.Slug, entity.IsFeatured, CreateMetadata(correlationId)), cancellationToken);

        return new CollectionView(entity.Id, entity.Name, entity.Slug, entity.Description, entity.IsFeatured);
    }

    public async Task DeleteCollectionAsync(Guid id, string correlationId, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Collections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException($"Collection '{id}' not found.");

        var isReferenced = await DbContext.ProductCollections.AnyAsync(x => x.CollectionId == id, cancellationToken);
        if (isReferenced)
        {
            throw new ConflictAppException($"Collection '{id}' is referenced by at least one product.");
        }

        DbContext.Collections.Remove(entity);
        await PublishAndFlushAsync(new CollectionDeletedV1(id, CreateMetadata(correlationId)), cancellationToken);
    }

    private async Task EnsureSlugUniqueAsync(string slug, Guid? currentId, CancellationToken cancellationToken)
    {
        var exists = await DbContext.Collections.AnyAsync(x => x.Slug == slug && (!currentId.HasValue || x.Id != currentId.Value), cancellationToken);
        if (exists)
        {
            throw new ConflictAppException($"Collection slug '{slug}' already exists.");
        }
    }
}
