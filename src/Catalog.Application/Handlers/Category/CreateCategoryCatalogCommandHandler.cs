using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Category;

public sealed class CreateCategoryCatalogCommandHandler(ICategoryCommandService categoryCommandService)
    : ICommandHandler<CreateCategoryCatalogCommand, CategoryView>
{
    public Task<CategoryView> HandleAsync(CreateCategoryCatalogCommand command, CancellationToken cancellationToken)
    {
        return categoryCommandService.CreateCategoryAsync(command.Category, cancellationToken);
    }
}
