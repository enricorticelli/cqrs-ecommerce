using Shared.BuildingBlocks.Contracts.Integration;

namespace Cart.Application.Views;

public sealed record CartView(Guid CartId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
