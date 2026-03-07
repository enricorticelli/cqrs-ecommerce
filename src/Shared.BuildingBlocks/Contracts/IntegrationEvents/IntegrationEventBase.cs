namespace Shared.BuildingBlocks.Contracts.IntegrationEvents;

public abstract record IntegrationEventBase(IntegrationEventMetadata Metadata) : IIntegrationEvent;
