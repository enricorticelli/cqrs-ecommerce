using Catalog.Application.Collections;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Commands;

public sealed record CreateCollectionCatalogCommand(CreateCollectionCommand Collection) : ICommand<CollectionView>;
