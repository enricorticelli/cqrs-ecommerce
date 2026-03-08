namespace Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;

public sealed record OrderCreatedV1(
    Guid OrderId,
    Guid CartId,
    Guid UserId,
    decimal TotalAmount,
    string Status,
    IntegrationEventMetadata Metadata) : IntegrationEventBase(Metadata);
