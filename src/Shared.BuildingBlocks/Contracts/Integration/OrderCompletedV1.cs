namespace Shared.BuildingBlocks.Contracts;

public sealed record OrderCompletedV1(Guid OrderId, string TrackingCode, string TransactionId);
