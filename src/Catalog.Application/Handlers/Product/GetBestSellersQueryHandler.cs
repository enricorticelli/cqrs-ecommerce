using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class GetBestSellersQueryHandler(IProductQueryService productQueryService)
    : IQueryHandler<GetBestSellersQuery, IReadOnlyList<ProductView>>
{
    public Task<IReadOnlyList<ProductView>> HandleAsync(GetBestSellersQuery query, CancellationToken cancellationToken)
    {
        return productQueryService.GetBestSellersAsync(cancellationToken);
    }
}
