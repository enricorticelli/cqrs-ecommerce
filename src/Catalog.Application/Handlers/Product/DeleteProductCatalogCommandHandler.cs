using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class DeleteProductCatalogCommandHandler(IProductCommandService productCommandService)
    : ICommandHandler<DeleteProductCatalogCommand, bool>
{
    public Task<bool> HandleAsync(DeleteProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return productCommandService.DeleteProductAsync(command.ProductId, cancellationToken);
    }
}
