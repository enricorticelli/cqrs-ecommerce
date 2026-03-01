namespace Shared.BuildingBlocks.Contracts;

public sealed record OrderFailedV1(Guid OrderId, string Reason);
