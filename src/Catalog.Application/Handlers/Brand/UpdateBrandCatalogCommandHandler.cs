using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Brand;

public sealed class UpdateBrandCatalogCommandHandler(IBrandCommandService brandCommandService)
    : ICommandHandler<UpdateBrandCatalogCommand, BrandView?>
{
    public Task<BrandView?> HandleAsync(UpdateBrandCatalogCommand command, CancellationToken cancellationToken)
    {
        return brandCommandService.UpdateBrandAsync(command.BrandId, command.Brand, cancellationToken);
    }
}
