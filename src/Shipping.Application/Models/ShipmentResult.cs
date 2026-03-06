namespace Shipping.Application.Models;

public sealed record ShipmentResult(Guid OrderId, string TrackingCode);
