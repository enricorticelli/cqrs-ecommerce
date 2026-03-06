using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record DeleteCategoryCatalogCommand(Guid CategoryId) : ICommand<bool>;
