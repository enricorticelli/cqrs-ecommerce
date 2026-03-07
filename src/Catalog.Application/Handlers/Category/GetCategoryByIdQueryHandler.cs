using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Category;

public sealed class GetCategoryByIdQueryHandler(ICategoryQueryService categoryQueryService)
    : IQueryHandler<GetCategoryByIdQuery, CategoryView?>
{
    public Task<CategoryView?> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        return categoryQueryService.GetCategoryByIdAsync(query.CategoryId, cancellationToken);
    }
}
