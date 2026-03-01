namespace Shipping.Application;

public sealed record ShipmentResult(Guid OrderId, string TrackingCode);
