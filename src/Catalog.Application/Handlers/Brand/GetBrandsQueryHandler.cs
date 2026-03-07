using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Brand;

public sealed class GetBrandsQueryHandler(IBrandQueryService brandQueryService)
    : IQueryHandler<GetBrandsQuery, IReadOnlyList<BrandView>>
{
    public Task<IReadOnlyList<BrandView>> HandleAsync(GetBrandsQuery query, CancellationToken cancellationToken)
    {
        return brandQueryService.GetBrandsAsync(query.Limit, query.Offset, query.SearchTerm, cancellationToken);
    }
}
