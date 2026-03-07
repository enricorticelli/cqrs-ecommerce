using Catalog.Application.Views;

namespace Catalog.Application.Abstractions.Collections;

public interface ICollectionService
{
    Task<IReadOnlyList<CollectionView>> GetCollectionsAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<CollectionView> GetCollectionAsync(Guid id, CancellationToken cancellationToken);

    Task<CollectionView> CreateCollectionAsync(string name, string slug, string description, bool isFeatured,
        string correlationId, CancellationToken cancellationToken);

    Task<CollectionView> UpdateCollectionAsync(Guid id, string name, string slug, string description, bool isFeatured,
        string correlationId, CancellationToken cancellationToken);

    Task DeleteCollectionAsync(Guid id, string correlationId, CancellationToken cancellationToken);
}
