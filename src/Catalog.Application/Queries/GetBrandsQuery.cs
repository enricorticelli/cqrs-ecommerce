using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Queries;

public sealed record GetBrandsQuery(int Limit, int Offset) : IQuery<IReadOnlyList<BrandView>>;
