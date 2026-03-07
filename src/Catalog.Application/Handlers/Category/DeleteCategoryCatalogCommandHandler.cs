using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Category;

public sealed class DeleteCategoryCatalogCommandHandler(ICategoryCommandService categoryCommandService)
    : ICommandHandler<DeleteCategoryCatalogCommand, bool>
{
    public Task<bool> HandleAsync(DeleteCategoryCatalogCommand command, CancellationToken cancellationToken)
    {
        return categoryCommandService.DeleteCategoryAsync(command.CategoryId, cancellationToken);
    }
}
