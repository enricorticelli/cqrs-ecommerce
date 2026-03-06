using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Queries;

public sealed record GetCollectionByIdQuery(Guid CollectionId) : IQuery<CollectionView?>;
