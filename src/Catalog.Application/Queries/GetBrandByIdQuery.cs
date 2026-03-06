using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Queries;

public sealed record GetBrandByIdQuery(Guid BrandId) : IQuery<BrandView?>;
