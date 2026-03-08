namespace Shipping.Infrastructure.Persistence.Entities;

public sealed class ShipmentNotificationSnapshot
{
    public Guid OrderId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
