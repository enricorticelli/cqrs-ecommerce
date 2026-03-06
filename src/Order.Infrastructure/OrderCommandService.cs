using Order.Application;
using Order.Domain;

namespace Order.Infrastructure;

public sealed class OrderCommandService(
    ICartSnapshotClient cartSnapshotClient,
    IWarehouseClient warehouseClient,
    IOrderStateStore orderStateStore,
    IOrderEventPublisher eventPublisher) : IOrderCommandService
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
        var customer = command.Customer ?? throw new InvalidOperationException("Customer details are required");
        var shippingAddress = command.ShippingAddress ?? throw new InvalidOperationException("Shipping address is required");
        var billingAddress = command.BillingAddress ?? throw new InvalidOperationException("Billing address is required");

        await orderStateStore.StartOrderAsync(
            orderId,
            command.CartId,
            command.UserId,
            command.IdentityType,
            command.AuthenticatedUserId,
            command.AnonymousId,
            customer,
            shippingAddress,
            billingAddress,
            cart.Items,
            cart.TotalAmount,
            cancellationToken);
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
        await eventPublisher.RequestPaymentAuthorizationAsync(orderId, command.UserId, cart.TotalAmount);
        return new OrderCreationResult(orderId, OrderStatus.StockReserved.ToString());
    }
}
