using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Collection;

public sealed class DeleteCollectionCatalogCommandHandler(ICollectionCommandService collectionCommandService)
    : ICommandHandler<DeleteCollectionCatalogCommand, bool>
{
    public Task<bool> HandleAsync(DeleteCollectionCatalogCommand command, CancellationToken cancellationToken)
    {
        return collectionCommandService.DeleteCollectionAsync(command.CollectionId, cancellationToken);
    }
}
