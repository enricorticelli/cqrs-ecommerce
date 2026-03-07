namespace Shared.BuildingBlocks.Contracts.IntegrationEvents;

public sealed record IntegrationEventMetadata(Guid EventId, DateTimeOffset OccurredAtUtc, string CorrelationId, string SourceContext);
