using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Infrastructure.Persistence.ReadModels;

namespace Catalog.Infrastructure.Services;

public sealed class CategoryQueryService(CatalogReadModelStore readModelStore) : ICategoryQueryService
{
    public async Task<IReadOnlyList<CategoryView>> GetCategoriesAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListCategoriesAsync(limit, offset, searchTerm, cancellationToken);
        return rows.Select(x => new CategoryView(x.Id, x.Name, x.Slug, x.Description)).ToArray();
    }

    public async Task<CategoryView?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await readModelStore.GetCategoryByIdAsync(id, cancellationToken);
        return row is null ? null : new CategoryView(row.Id, row.Name, row.Slug, row.Description);
    }
}
