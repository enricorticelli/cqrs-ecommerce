using Shared.BuildingBlocks.Cqrs;

namespace Catalog.Application;

public sealed record GetCategoriesQuery(int Limit, int Offset) : IQuery<IReadOnlyList<CategoryView>>;
