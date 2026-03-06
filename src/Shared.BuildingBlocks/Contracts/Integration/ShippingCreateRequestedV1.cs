namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record ShippingCreateRequestedV1(Guid OrderId, Guid UserId, IReadOnlyList<OrderItemDto> Items);
