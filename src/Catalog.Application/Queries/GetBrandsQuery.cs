using Shared.BuildingBlocks.Cqrs;

namespace Catalog.Application;

public sealed record GetBrandsQuery(int Limit, int Offset) : IQuery<IReadOnlyList<BrandView>>;
