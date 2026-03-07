using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class GetProductsQueryHandler(IProductQueryService productQueryService)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductView>>
{
    public Task<IReadOnlyList<ProductView>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        return productQueryService.GetProductsAsync(query.Limit, query.Offset, cancellationToken);
    }
}
