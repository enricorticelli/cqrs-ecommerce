using Order.Application.Models;

namespace Order.Application.Abstractions;

public interface IOrderStateReader
{
    Task<OrderAggregateState?> GetAsync(Guid orderId, CancellationToken cancellationToken);
}
