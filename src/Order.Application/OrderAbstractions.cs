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

public sealed record StockReservationDecision(bool Reserved, string? Reason = null);
public sealed record PaymentAuthorizationDecision(bool Authorized, string TransactionId = "");

public interface ICartSnapshotClient
{
    Task<CartSnapshot?> GetCartAsync(Guid cartId, CancellationToken cancellationToken);
}

public interface IWarehouseClient
{
    Task<StockReservationDecision> ReserveStockAsync(Guid orderId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken);
}

public interface IPaymentClient
{
    Task<PaymentAuthorizationDecision> AuthorizeAsync(Guid orderId, Guid userId, decimal amount, CancellationToken cancellationToken);
}

public interface IShippingClient
{
    Task<string> CreateShipmentAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken);
}

public interface IOrderStateStore
{
    Task StartOrderAsync(Guid orderId, Guid cartId, Guid userId, IReadOnlyList<OrderItemDto> items, decimal totalAmount, CancellationToken cancellationToken);
    Task<OrderAggregateState?> GetAsync(Guid orderId, CancellationToken cancellationToken);
    Task<OrderView?> GetOrderViewAsync(Guid orderId, CancellationToken cancellationToken);
    Task MarkStockReservedAsync(Guid orderId, CancellationToken cancellationToken);
    Task MarkPaymentAuthorizedAsync(Guid orderId, string transactionId, CancellationToken cancellationToken);
    Task MarkCompletedAsync(Guid orderId, string trackingCode, string transactionId, CancellationToken cancellationToken);
    Task MarkFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken);
}

public interface IOrderEventPublisher
{
    Task PublishOrderPlacedAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items, decimal totalAmount);
    Task PublishOrderFailedAsync(Guid orderId, string reason);
    Task PublishOrderCompletedAsync(Guid orderId, string trackingCode, string transactionId);
    Task RequestPaymentAuthorizationAsync(Guid orderId, Guid userId, decimal amount);
    Task RequestShippingAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items);
}

public interface IOrderWorkflowProcessor
{
    Task HandleStockReservedAsync(StockReservedV1 message, CancellationToken cancellationToken);
    Task HandleStockRejectedAsync(StockRejectedV1 message, CancellationToken cancellationToken);
    Task HandlePaymentAuthorizedAsync(PaymentAuthorizedV1 message, CancellationToken cancellationToken);
    Task HandlePaymentFailedAsync(PaymentFailedV1 message, CancellationToken cancellationToken);
    Task HandleShippingCreatedAsync(ShippingCreatedV1 message, CancellationToken cancellationToken);
}
