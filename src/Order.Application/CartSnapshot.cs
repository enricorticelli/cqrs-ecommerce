using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public sealed record CartSnapshot(Guid CartId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
