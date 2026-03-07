using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Application.Views;

public sealed record OrderView(
    Guid Id,
    Guid CartId,
    Guid UserId,
    string IdentityType,
    string PaymentMethod,
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
