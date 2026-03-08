namespace Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;

public sealed record OrderCompletedForCommunicationV1(
    Guid OrderId,
    Guid UserId,
    string CustomerEmail,
    decimal TotalAmount,
    IntegrationEventMetadata Metadata) : IntegrationEventBase(Metadata);
