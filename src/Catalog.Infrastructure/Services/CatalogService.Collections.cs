using Catalog.Application;
using Catalog.Domain;
using Marten;

namespace Catalog.Infrastructure;

public sealed partial class CatalogService
{
    public async Task<IReadOnlyList<CollectionDocument>> GetCollectionsAsync(CancellationToken cancellationToken)
    {
        return await _querySession.Query<CollectionDocument>()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<CollectionDocument?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _querySession.LoadAsync<CollectionDocument>(id, cancellationToken);
    }

    public async Task<CollectionDocument> CreateCollectionAsync(CreateCollectionCommand command, CancellationToken cancellationToken)
    {
        var collection = new CollectionDocument
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            IsFeatured = command.IsFeatured
        };

        _documentSession.Store(collection);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return collection;
    }

    public async Task<CollectionDocument?> UpdateCollectionAsync(Guid id, UpdateCollectionCommand command, CancellationToken cancellationToken)
    {
        var existing = await _documentSession.LoadAsync<CollectionDocument>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new CollectionDocument
        {
            Id = id,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            IsFeatured = command.IsFeatured
        };

        _documentSession.Store(updated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteCollectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await _documentSession.LoadAsync<CollectionDocument>(id, cancellationToken);
        if (collection is null)
        {
            return false;
        }

        _documentSession.Delete<CollectionDocument>(id);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
