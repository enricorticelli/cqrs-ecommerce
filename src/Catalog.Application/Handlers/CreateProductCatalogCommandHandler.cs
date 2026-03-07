using Catalog.Application.Abstractions;
using Catalog.Application.Products;
using Catalog.Application.Views;

namespace Catalog.Application.Handlers;

public sealed class CreateProductCatalogCommandHandler(IProductCommandService productCommandService)
{
    public Task<ProductView> HandleAsync(CreateProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return productCommandService.CreateProductAsync(command.Payload, cancellationToken);
    }
}
