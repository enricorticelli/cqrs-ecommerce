using Catalog.Application.Views;

namespace Catalog.Application.Abstractions.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryView>> GetCategoriesAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<CategoryView> GetCategoryAsync(Guid id, CancellationToken cancellationToken);
    Task<CategoryView> CreateCategoryAsync(string name, string slug, string description, string correlationId, CancellationToken cancellationToken);
    Task<CategoryView> UpdateCategoryAsync(Guid id, string name, string slug, string description, string correlationId, CancellationToken cancellationToken);
    Task DeleteCategoryAsync(Guid id, string correlationId, CancellationToken cancellationToken);
}
