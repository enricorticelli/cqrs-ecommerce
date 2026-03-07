using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Infrastructure.Persistence.ReadModels;

namespace Catalog.Infrastructure.Services;

public sealed class CollectionQueryService(CatalogReadModelStore readModelStore) : ICollectionQueryService
{
    public async Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListCollectionsAsync(limit, offset, searchTerm, cancellationToken);
        return rows.Select(x => new CollectionView(x.Id, x.Name, x.Slug, x.Description, x.IsFeatured)).ToArray();
    }

    public async Task<CollectionView?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await readModelStore.GetCollectionByIdAsync(id, cancellationToken);
        return row is null ? null : new CollectionView(row.Id, row.Name, row.Slug, row.Description, row.IsFeatured);
    }
}
