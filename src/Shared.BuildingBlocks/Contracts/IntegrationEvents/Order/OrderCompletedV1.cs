namespace Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;

public sealed record OrderCompletedV1(
    Guid OrderId,
    Guid CartId,
    Guid UserId,
    string TrackingCode,
    string TransactionId,
    IntegrationEventMetadata Metadata) : IntegrationEventBase(Metadata);
