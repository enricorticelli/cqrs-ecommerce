using Order.Application.Abstractions.Queries;
using Order.Application.Abstractions.Repositories;
using Order.Application.Views;
using Shared.BuildingBlocks.Exceptions;
using Shared.BuildingBlocks.Mapping;

namespace Order.Application.Services;

public sealed class OrderQueryService(
    IOrderRepository orderRepository,
    IViewMapper<Order.Domain.Entities.Order, OrderView> mapper) : IOrderQueryService
{
    public async Task<IReadOnlyList<OrderView>> ListAsync(CancellationToken cancellationToken)
    {
        var orders = await orderRepository.ListAsync(cancellationToken);
        return orders.Select(mapper.Map).ToArray();
    }

    public async Task<OrderView> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundAppException($"Order '{id}' not found.");

        return mapper.Map(order);
    }
}
