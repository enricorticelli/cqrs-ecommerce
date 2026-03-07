using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface ICollectionQueryService
{
    Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken);
    Task<CollectionView?> GetCollectionByIdAsync(Guid id, CancellationToken cancellationToken);
}
