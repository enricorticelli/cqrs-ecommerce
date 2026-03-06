namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record StockReserveRequestedV1(Guid OrderId, IReadOnlyList<OrderItemDto> Items);
