using Shared.BuildingBlocks.Contracts.IntegrationEvents;

namespace Shared.BuildingBlocks.Contracts.IntegrationEvents.Catalog;

public sealed record ProductDeletedV1(Guid ProductId, IntegrationEventMetadata Metadata) : IntegrationEventBase(Metadata);
