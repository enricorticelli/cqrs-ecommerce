namespace Order.Application;

public sealed record OrderCreationResult(Guid OrderId, string Status);

public sealed record OrderView(
    Guid Id,
    Guid CartId,
    Guid UserId,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<Shared.BuildingBlocks.Contracts.OrderItemDto> Items,
    string TrackingCode,
    string TransactionId,
    string FailureReason);

public interface IOrderService
{
    Task<OrderCreationResult?> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken);
    Task<OrderView?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken);
}
