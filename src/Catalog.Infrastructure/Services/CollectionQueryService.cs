using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class CollectionQueryService(IQuerySession querySession) : ICollectionQueryService
{
    public async Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var collections = await querySession.Query<CollectionAggregate>()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return collections.Select(MapToView).ToArray();
    }

    public async Task<CollectionView?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var collection = await querySession.LoadAsync<CollectionAggregate>(id, cancellationToken);
        return collection is null || collection.IsDeleted ? null : MapToView(collection);
    }

    private static CollectionView MapToView(CollectionAggregate collection)
    {
        return new CollectionView(collection.Id, collection.Name, collection.Slug, collection.Description, collection.IsFeatured);
    }
}
