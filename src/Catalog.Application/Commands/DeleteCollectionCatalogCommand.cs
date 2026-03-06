using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record DeleteCollectionCatalogCommand(Guid CollectionId) : ICommand<bool>;
