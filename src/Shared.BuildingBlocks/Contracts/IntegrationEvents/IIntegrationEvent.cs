namespace Shared.BuildingBlocks.Contracts.IntegrationEvents;

public interface IIntegrationEvent
{
    IntegrationEventMetadata Metadata { get; }
}
