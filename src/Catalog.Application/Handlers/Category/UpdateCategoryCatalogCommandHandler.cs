using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Category;

public sealed class UpdateCategoryCatalogCommandHandler(ICategoryCommandService categoryCommandService)
    : ICommandHandler<UpdateCategoryCatalogCommand, CategoryView?>
{
    public Task<CategoryView?> HandleAsync(UpdateCategoryCatalogCommand command, CancellationToken cancellationToken)
    {
        return categoryCommandService.UpdateCategoryAsync(command.CategoryId, command.Category, cancellationToken);
    }
}
