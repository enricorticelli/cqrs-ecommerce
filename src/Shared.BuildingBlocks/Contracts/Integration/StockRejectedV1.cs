namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record StockRejectedV1(Guid OrderId, string Reason);
