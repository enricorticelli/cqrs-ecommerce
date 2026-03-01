namespace Shared.BuildingBlocks.Contracts;

public sealed record CartCheckedOutV1(Guid CartId, Guid OrderId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
