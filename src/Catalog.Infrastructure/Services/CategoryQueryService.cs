using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class CategoryQueryService(IQuerySession querySession) : ICategoryQueryService
{
    public async Task<IReadOnlyList<CategoryView>> GetCategoriesAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var categories = await querySession.Query<CategoryAggregate>()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return categories.Select(MapToView).ToArray();
    }

    public async Task<CategoryView?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await querySession.LoadAsync<CategoryAggregate>(id, cancellationToken);
        return category is null || category.IsDeleted ? null : MapToView(category);
    }

    private static CategoryView MapToView(CategoryAggregate category)
    {
        return new CategoryView(category.Id, category.Name, category.Slug, category.Description);
    }
}
