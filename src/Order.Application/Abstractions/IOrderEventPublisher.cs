using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public interface IOrderEventPublisher
{
    Task PublishOrderPlacedAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items, decimal totalAmount);
    Task PublishOrderFailedAsync(Guid orderId, string reason);
    Task PublishOrderCompletedAsync(Guid orderId, string trackingCode, string transactionId);
    Task RequestPaymentAuthorizationAsync(Guid orderId, Guid userId, decimal amount);
    Task RequestShippingAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items);
}
