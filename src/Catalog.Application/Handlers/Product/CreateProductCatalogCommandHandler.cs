using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class CreateProductCatalogCommandHandler(IProductCommandService productCommandService)
    : ICommandHandler<CreateProductCatalogCommand, ProductView?>
{
    public Task<ProductView?> HandleAsync(CreateProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return productCommandService.CreateProductAsync(command.Product, cancellationToken);
    }
}
