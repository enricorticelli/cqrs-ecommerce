using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Brand;

public sealed class GetBrandByIdQueryHandler(IBrandQueryService brandQueryService)
    : IQueryHandler<GetBrandByIdQuery, BrandView?>
{
    public Task<BrandView?> HandleAsync(GetBrandByIdQuery query, CancellationToken cancellationToken)
    {
        return brandQueryService.GetBrandByIdAsync(query.BrandId, cancellationToken);
    }
}
