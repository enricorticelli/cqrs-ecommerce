using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class GetProductByIdQueryHandler(IProductQueryService productQueryService)
    : IQueryHandler<GetProductByIdQuery, ProductView?>
{
    public Task<ProductView?> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        return productQueryService.GetProductByIdAsync(query.ProductId, cancellationToken);
    }
}
