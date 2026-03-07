using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class UpdateProductCatalogCommandHandler(IProductCommandService productCommandService)
    : ICommandHandler<UpdateProductCatalogCommand, ProductView?>
{
    public Task<ProductView?> HandleAsync(UpdateProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return productCommandService.UpdateProductAsync(command.ProductId, command.Product, cancellationToken);
    }
}
