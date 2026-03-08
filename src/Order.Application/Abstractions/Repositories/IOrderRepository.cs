using OrderEntity = Order.Domain.Entities.Order;

namespace Order.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task<IReadOnlyList<OrderEntity>> ListAsync(CancellationToken cancellationToken);
    Task<OrderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(OrderEntity order);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
