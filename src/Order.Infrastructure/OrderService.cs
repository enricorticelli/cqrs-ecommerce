using Order.Application;
using Order.Domain;

namespace Order.Infrastructure;

public sealed class OrderService(
    ICartSnapshotClient cartSnapshotClient,
    IWarehouseClient warehouseClient,
    IPaymentClient paymentClient,
    IShippingClient shippingClient,
    IOrderStateStore orderStateStore,
    IOrderEventPublisher eventPublisher) : IOrderService
{
    public async Task<OrderCreationResult?> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var cart = await cartSnapshotClient.GetCartAsync(command.CartId, cancellationToken);
        if (cart is null)
        {
            return null;
        }

        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty or unreadable");
        }

        var orderId = Guid.NewGuid();
        await orderStateStore.StartOrderAsync(orderId, command.CartId, command.UserId, cart.Items, cart.TotalAmount, cancellationToken);
        await eventPublisher.PublishOrderPlacedAsync(orderId, command.UserId, cart.Items, cart.TotalAmount);

        var stockReservation = await warehouseClient.ReserveStockAsync(orderId, cart.Items, cancellationToken);
        if (!stockReservation.Reserved)
        {
            var reason = stockReservation.Reason ?? "Stock not available";
            await orderStateStore.MarkFailedAsync(orderId, reason, cancellationToken);
            await eventPublisher.PublishOrderFailedAsync(orderId, reason);
            return new OrderCreationResult(orderId, OrderStatus.Failed.ToString());
        }

        await orderStateStore.MarkStockReservedAsync(orderId, cancellationToken);

        var payment = await paymentClient.AuthorizeAsync(orderId, command.UserId, cart.TotalAmount, cancellationToken);
        if (!payment.Authorized)
        {
            const string reason = "Payment declined";
            await orderStateStore.MarkFailedAsync(orderId, reason, cancellationToken);
            await eventPublisher.PublishOrderFailedAsync(orderId, reason);
            return new OrderCreationResult(orderId, OrderStatus.Failed.ToString());
        }

        await orderStateStore.MarkPaymentAuthorizedAsync(orderId, payment.TransactionId, cancellationToken);

        var trackingCode = await shippingClient.CreateShipmentAsync(orderId, command.UserId, cart.Items, cancellationToken);
        await orderStateStore.MarkCompletedAsync(orderId, trackingCode, payment.TransactionId, cancellationToken);
        await eventPublisher.PublishOrderCompletedAsync(orderId, trackingCode, payment.TransactionId);

        return new OrderCreationResult(orderId, OrderStatus.Completed.ToString());
    }

    public Task<OrderView?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return orderStateStore.GetOrderViewAsync(orderId, cancellationToken);
    }
}
