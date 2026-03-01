using Shared.BuildingBlocks.Contracts;

namespace Cart.Application;

public sealed record CartView(Guid CartId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
