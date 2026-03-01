namespace Shared.BuildingBlocks.Contracts;

public sealed record PaymentFailedV1(Guid OrderId, string Reason);
