using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface ICategoryQueryService
{
    Task<IReadOnlyList<CategoryView>> GetCategoriesAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken);
    Task<CategoryView?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
}
