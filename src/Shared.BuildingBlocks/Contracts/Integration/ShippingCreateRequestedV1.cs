namespace Shared.BuildingBlocks.Contracts;

public sealed record ShippingCreateRequestedV1(Guid OrderId, Guid UserId, IReadOnlyList<OrderItemDto> Items);
