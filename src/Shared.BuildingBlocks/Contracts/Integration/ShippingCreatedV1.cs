namespace Shared.BuildingBlocks.Contracts;

public sealed record ShippingCreatedV1(Guid OrderId, string TrackingCode);
