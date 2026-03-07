using Catalog.Application.Abstractions;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Category;

public sealed class GetCategoriesQueryHandler(ICategoryQueryService categoryQueryService)
    : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryView>>
{
    public Task<IReadOnlyList<CategoryView>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        return categoryQueryService.GetCategoriesAsync(query.Limit, query.Offset, query.SearchTerm, cancellationToken);
    }
}
