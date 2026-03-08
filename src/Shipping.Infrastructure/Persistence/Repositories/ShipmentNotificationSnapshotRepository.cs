using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions.Repositories;
using Shipping.Infrastructure.Persistence.Entities;

namespace Shipping.Infrastructure.Persistence.Repositories;

public sealed class ShipmentNotificationSnapshotRepository(ShippingDbContext dbContext) : IShipmentNotificationSnapshotRepository
{
    public async Task UpsertCustomerEmailAsync(Guid orderId, string customerEmail, CancellationToken cancellationToken)
    {
        var snapshot = await dbContext.ShipmentNotificationSnapshots
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);

        if (snapshot is null)
        {
            dbContext.ShipmentNotificationSnapshots.Add(new ShipmentNotificationSnapshot
            {
                OrderId = orderId,
                CustomerEmail = customerEmail,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            });
            return;
        }

        snapshot.CustomerEmail = customerEmail;
        snapshot.UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public async Task<string?> GetCustomerEmailByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.ShipmentNotificationSnapshots
            .Where(x => x.OrderId == orderId)
            .Select(x => x.CustomerEmail)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
