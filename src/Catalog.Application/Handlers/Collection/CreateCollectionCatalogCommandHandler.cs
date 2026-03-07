using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Collection;

public sealed class CreateCollectionCatalogCommandHandler(ICollectionCommandService collectionCommandService)
    : ICommandHandler<CreateCollectionCatalogCommand, CollectionView>
{
    public Task<CollectionView> HandleAsync(CreateCollectionCatalogCommand command, CancellationToken cancellationToken)
    {
        return collectionCommandService.CreateCollectionAsync(command.Collection, cancellationToken);
    }
}
