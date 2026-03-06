using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public sealed record OrderView(
    Guid Id,
    Guid CartId,
    Guid UserId,
    string IdentityType,
    Guid? AuthenticatedUserId,
    Guid? AnonymousId,
    OrderCustomerDetails Customer,
    OrderAddress ShippingAddress,
    OrderAddress BillingAddress,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items,
    string TrackingCode,
    string TransactionId,
    string FailureReason);
