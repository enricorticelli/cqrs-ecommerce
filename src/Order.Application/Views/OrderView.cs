using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public sealed record OrderView(
    Guid Id,
    Guid CartId,
    Guid UserId,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items,
    string TrackingCode,
    string TransactionId,
    string FailureReason);
