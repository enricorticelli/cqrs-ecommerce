namespace Shipping.Api.Contracts;

public sealed record CreateShipmentResponse(Guid OrderId, string TrackingCode);
