using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers;

public sealed class ProductRequestHandlers(ICatalogService catalogService) :
    IQueryHandler<GetProductsQuery, IReadOnlyList<ProductView>>,
    IQueryHandler<GetNewArrivalsQuery, IReadOnlyList<ProductView>>,
    IQueryHandler<GetBestSellersQuery, IReadOnlyList<ProductView>>,
    IQueryHandler<GetProductByIdQuery, ProductView?>,
    ICommandHandler<CreateProductCatalogCommand, ProductView?>,
    ICommandHandler<UpdateProductCatalogCommand, ProductView?>,
    ICommandHandler<DeleteProductCatalogCommand, bool>
{
    public Task<IReadOnlyList<ProductView>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        return catalogService.GetProductsAsync(query.Limit, query.Offset, cancellationToken);
    }

    public Task<IReadOnlyList<ProductView>> HandleAsync(GetNewArrivalsQuery query, CancellationToken cancellationToken)
    {
        return catalogService.GetNewArrivalsAsync(cancellationToken);
    }

    public Task<IReadOnlyList<ProductView>> HandleAsync(GetBestSellersQuery query, CancellationToken cancellationToken)
    {
        return catalogService.GetBestSellersAsync(cancellationToken);
    }

    public Task<ProductView?> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        return catalogService.GetProductByIdAsync(query.ProductId, cancellationToken);
    }

    public Task<ProductView?> HandleAsync(CreateProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return catalogService.CreateProductAsync(command.Product, cancellationToken);
    }

    public Task<ProductView?> HandleAsync(UpdateProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return catalogService.UpdateProductAsync(command.ProductId, command.Product, cancellationToken);
    }

    public Task<bool> HandleAsync(DeleteProductCatalogCommand command, CancellationToken cancellationToken)
    {
        return catalogService.DeleteProductAsync(command.ProductId, cancellationToken);
    }
}
