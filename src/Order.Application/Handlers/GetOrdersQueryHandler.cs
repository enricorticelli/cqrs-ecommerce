using Order.Application.Abstractions;
using Order.Application.Queries;
using Order.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Handlers;

public sealed class GetOrdersQueryHandler(IOrderQueryService orderQueryService)
    : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderView>>
{
    public Task<IReadOnlyList<OrderView>> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        return orderQueryService.GetOrdersAsync(query.Limit, query.Offset, cancellationToken);
    }
}
