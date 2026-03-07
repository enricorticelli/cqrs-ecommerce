using Catalog.Application.Collections;
using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface ICollectionCommandService
{
    Task<CollectionView> CreateCollectionAsync(CreateCollectionCommand command, CancellationToken cancellationToken);
    Task<CollectionView?> UpdateCollectionAsync(Guid id, UpdateCollectionCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteCollectionAsync(Guid id, CancellationToken cancellationToken);
}
