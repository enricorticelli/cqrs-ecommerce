using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Brand;

public sealed class DeleteBrandCatalogCommandHandler(IBrandCommandService brandCommandService)
    : ICommandHandler<DeleteBrandCatalogCommand, bool>
{
    public Task<bool> HandleAsync(DeleteBrandCatalogCommand command, CancellationToken cancellationToken)
    {
        return brandCommandService.DeleteBrandAsync(command.BrandId, cancellationToken);
    }
}
