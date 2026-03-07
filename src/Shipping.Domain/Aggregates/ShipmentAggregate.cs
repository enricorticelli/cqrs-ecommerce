namespace Shipping.Domain.Aggregates;

public sealed class ShipmentAggregate
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public required string TrackingCode { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public DateTimeOffset? DeliveredAtUtc { get; set; }
}
