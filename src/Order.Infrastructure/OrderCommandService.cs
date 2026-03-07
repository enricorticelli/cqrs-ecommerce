using Order.Application;
using Order.Application.Abstractions;
using Order.Application.Views;
using Order.Domain.Enums;

namespace Order.Infrastructure;

public sealed class OrderCommandService(
    IOrderStateStore orderStateStore,
    IOrderEventPublisher eventPublisher) : IOrderCommandService
{
    public async Task<OrderCreationResult?> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();
        var customer = command.Customer ?? throw new InvalidOperationException("Customer details are required");
        var shippingAddress = command.ShippingAddress ?? throw new InvalidOperationException("Shipping address is required");
        var billingAddress = command.BillingAddress ?? throw new InvalidOperationException("Billing address is required");

        await orderStateStore.StartOrderAsync(
            orderId,
            command.CartId,
            command.UserId,
            command.IdentityType,
            command.PaymentMethod,
            command.AuthenticatedUserId,
            command.AnonymousId,
            customer,
            shippingAddress,
            billingAddress,
            command.Items,
            command.TotalAmount,
            cancellationToken);
        await eventPublisher.PublishOrderPlacedAsync(orderId, command.UserId, command.Items, command.TotalAmount);
        return new OrderCreationResult(orderId, OrderStatus.Pending.ToString());
    }
}
