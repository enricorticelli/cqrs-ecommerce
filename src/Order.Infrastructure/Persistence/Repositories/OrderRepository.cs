using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions.Repositories;

namespace Order.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<IReadOnlyList<Order.Domain.Entities.Order>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(x => x.Items)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Order.Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Add(Order.Domain.Entities.Order order)
    {
        dbContext.Orders.Add(order);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
