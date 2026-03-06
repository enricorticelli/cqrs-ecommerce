using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public interface IOrderStateStore
{
    Task StartOrderAsync(
        Guid orderId,
        Guid cartId,
        Guid userId,
        string identityType,
        Guid? authenticatedUserId,
        Guid? anonymousId,
        OrderCustomerDetails customer,
        OrderAddress shippingAddress,
        OrderAddress billingAddress,
        IReadOnlyList<OrderItemDto> items,
        decimal totalAmount,
        CancellationToken cancellationToken);
    Task MarkStockReservedAsync(Guid orderId, CancellationToken cancellationToken);
    Task MarkPaymentAuthorizedAsync(Guid orderId, string transactionId, CancellationToken cancellationToken);
    Task MarkCompletedAsync(Guid orderId, string trackingCode, string transactionId, CancellationToken cancellationToken);
    Task MarkFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken);
}
