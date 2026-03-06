namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record OrderPlacedV1(Guid OrderId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
