using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Collection;

public sealed class UpdateCollectionCatalogCommandHandler(ICollectionCommandService collectionCommandService)
    : ICommandHandler<UpdateCollectionCatalogCommand, CollectionView?>
{
    public Task<CollectionView?> HandleAsync(UpdateCollectionCatalogCommand command, CancellationToken cancellationToken)
    {
        return collectionCommandService.UpdateCollectionAsync(command.CollectionId, command.Collection, cancellationToken);
    }
}
