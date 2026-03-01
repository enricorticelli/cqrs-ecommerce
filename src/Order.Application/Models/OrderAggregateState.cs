using Order.Domain;
using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public sealed record OrderAggregateState(
    Guid OrderId,
    Guid CartId,
    Guid UserId,
    OrderStatus Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items,
    string TransactionId,
    string TrackingCode,
    string FailureReason);
