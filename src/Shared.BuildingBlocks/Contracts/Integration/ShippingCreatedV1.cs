namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record ShippingCreatedV1(Guid OrderId, string TrackingCode);
