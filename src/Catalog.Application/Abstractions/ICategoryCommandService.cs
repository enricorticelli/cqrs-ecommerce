using Catalog.Application.Categories;
using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface ICategoryCommandService
{
    Task<CategoryView> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken);
    Task<CategoryView?> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken);
}
