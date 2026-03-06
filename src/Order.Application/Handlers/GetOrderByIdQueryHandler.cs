using Order.Application.Abstractions;
using Order.Application.Queries;
using Order.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Handlers;

public sealed class GetOrderByIdQueryHandler(IOrderQueryService orderQueryService)
    : IQueryHandler<GetOrderByIdQuery, OrderView?>
{
    public Task<OrderView?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        return orderQueryService.GetOrderAsync(query.OrderId, cancellationToken);
    }
}
