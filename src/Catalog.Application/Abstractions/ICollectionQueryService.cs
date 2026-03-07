using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface ICollectionQueryService
{
    Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(int limit, int offset, CancellationToken cancellationToken);
    Task<CollectionView?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken);
}
