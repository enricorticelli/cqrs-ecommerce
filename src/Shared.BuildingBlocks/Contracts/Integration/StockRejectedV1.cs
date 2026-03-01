namespace Shared.BuildingBlocks.Contracts;

public sealed record StockRejectedV1(Guid OrderId, string Reason);
