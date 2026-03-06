namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record OrderFailedV1(Guid OrderId, string Reason);
