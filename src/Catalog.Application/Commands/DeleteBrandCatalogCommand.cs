using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record DeleteBrandCatalogCommand(Guid BrandId) : ICommand<bool>;
