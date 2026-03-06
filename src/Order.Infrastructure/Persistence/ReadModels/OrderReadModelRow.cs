using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.Persistence.ReadModels;

public sealed record OrderReadModelRow(
    Guid OrderId,
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
    string TransactionId,
    string TrackingCode,
    string FailureReason);
