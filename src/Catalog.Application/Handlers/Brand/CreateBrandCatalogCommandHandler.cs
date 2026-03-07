using Catalog.Application.Abstractions;
using Catalog.Application.Commands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Handlers.Brand;

public sealed class CreateBrandCatalogCommandHandler(IBrandCommandService brandCommandService)
    : ICommandHandler<CreateBrandCatalogCommand, BrandView>
{
    public Task<BrandView> HandleAsync(CreateBrandCatalogCommand command, CancellationToken cancellationToken)
    {
        return brandCommandService.CreateBrandAsync(command.Brand, cancellationToken);
    }
}
