namespace Shipping.Application.Models;

public sealed record ShipmentView(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    string TrackingCode,
    string Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? DeliveredAtUtc);