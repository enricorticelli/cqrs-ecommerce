using Shared.BuildingBlocks.Cqrs;

namespace Catalog.Application;

public sealed record GetProductsQuery(int Limit, int Offset) : IQuery<IReadOnlyList<ProductView>>;
