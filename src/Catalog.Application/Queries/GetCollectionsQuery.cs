using Shared.BuildingBlocks.Cqrs;

namespace Catalog.Application;

public sealed record GetCollectionsQuery(int Limit, int Offset) : IQuery<IReadOnlyList<CollectionView>>;
