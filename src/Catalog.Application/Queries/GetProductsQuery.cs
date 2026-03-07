using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Queries;

public sealed record GetProductsQuery(int Limit, int Offset, string? SearchTerm) : IQuery<IReadOnlyList<ProductView>>;
