using Catalog.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Application.Queries;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryView?>;
