namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record PaymentFailedV1(Guid OrderId, string Reason);
