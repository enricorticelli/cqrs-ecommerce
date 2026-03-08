using Order.Application.Views;

namespace Order.Application.Abstractions.Queries;

public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderView>> ListAsync(CancellationToken cancellationToken);
    Task<OrderView> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
