using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Application;

public sealed record CartSnapshot(Guid CartId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
