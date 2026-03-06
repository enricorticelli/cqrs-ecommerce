using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record DeleteProductCatalogCommand(Guid ProductId) : ICommand<bool>;
