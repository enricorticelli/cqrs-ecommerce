namespace Shared.BuildingBlocks.Contracts;

public sealed record OrderPlacedV1(Guid OrderId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
