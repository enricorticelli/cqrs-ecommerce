namespace Shared.BuildingBlocks.Contracts.IntegrationEvents.Shipping;

public sealed record ShipmentInTransitForCommunicationV1(
    Guid OrderId,
    string TrackingCode,
    string CustomerEmail,
    IntegrationEventMetadata Metadata) : IntegrationEventBase(Metadata);
