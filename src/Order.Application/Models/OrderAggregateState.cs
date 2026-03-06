using Order.Domain;
using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public sealed record OrderAggregateState(
    Guid OrderId,
    Guid CartId,
    Guid UserId,
    string IdentityType,
    Guid? AuthenticatedUserId,
    Guid? AnonymousId,
    OrderCustomerDetails Customer,
    OrderAddress ShippingAddress,
    OrderAddress BillingAddress,
    OrderStatus Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items,
    string TransactionId,
    string TrackingCode,
    string FailureReason);
