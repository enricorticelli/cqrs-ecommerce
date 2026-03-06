using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Domain.Events;

public sealed record OrderPlacedDomain(
    Guid OrderId,
    Guid CartId,
    Guid UserId,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalAmount,
    string? IdentityType = null,
    Guid? AuthenticatedUserId = null,
    Guid? AnonymousId = null,
    OrderCustomerDetails? Customer = null,
    OrderAddress? ShippingAddress = null,
    OrderAddress? BillingAddress = null);
