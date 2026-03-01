namespace Shared.BuildingBlocks.Contracts;

public sealed record StockReserveRequestedV1(Guid OrderId, IReadOnlyList<OrderItemDto> Items);
