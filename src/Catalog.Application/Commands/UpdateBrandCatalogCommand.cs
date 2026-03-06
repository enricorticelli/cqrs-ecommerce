using Catalog.Application.Brands;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record UpdateBrandCatalogCommand(Guid BrandId, UpdateBrandCommand Brand) : ICommand<BrandView?>;
