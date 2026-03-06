namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record OrderCompletedV1(Guid OrderId, string TrackingCode, string TransactionId);
