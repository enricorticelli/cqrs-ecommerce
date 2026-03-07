using Order.Application.Views;

namespace Order.Application.Abstractions;

public interface IOrderQueryService
{
    Task<OrderView?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderView>> GetOrdersAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken);
}
