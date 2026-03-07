using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Product;

public sealed class GetNewArrivalsQueryHandler(IProductQueryService productQueryService)
    : IQueryHandler<GetNewArrivalsQuery, IReadOnlyList<ProductView>>
{
    public Task<IReadOnlyList<ProductView>> HandleAsync(GetNewArrivalsQuery query, CancellationToken cancellationToken)
    {
        return productQueryService.GetNewArrivalsAsync(cancellationToken);
    }
}
