namespace Shipping.Application.Abstractions.Repositories;

public interface IShipmentNotificationSnapshotRepository
{
    Task UpsertCustomerEmailAsync(Guid orderId, string customerEmail, CancellationToken cancellationToken);
    Task<string?> GetCustomerEmailByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}
