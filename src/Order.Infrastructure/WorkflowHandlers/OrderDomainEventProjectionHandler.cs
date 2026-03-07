using Order.Domain.Events;
using Order.Infrastructure.Persistence.ReadModels;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderDomainEventProjectionHandler
{
    public static Task Handle(OrderPlacedDomain message, IOrderReadModelStore readModelStore, CancellationToken cancellationToken)
    {
        var row = new OrderReadModelRow(
            message.OrderId,
            message.CartId,
            message.UserId,
            string.IsNullOrWhiteSpace(message.IdentityType) ? OrderIdentityTypes.Anonymous : message.IdentityType,
            PaymentMethodTypes.IsSupported(message.PaymentMethod) ? message.PaymentMethod : PaymentMethodTypes.StripeCard,
            message.AuthenticatedUserId,
            message.AnonymousId,
            message.Customer ?? OrderCustomerDetails.Empty,
            message.ShippingAddress ?? OrderAddress.Empty,
            message.BillingAddress ?? OrderAddress.Empty,
            "Pending",
            message.TotalAmount,
            message.Items,
            string.Empty,
            string.Empty,
            string.Empty);

        return readModelStore.UpsertAsync(row, cancellationToken);
    }

    public static async Task Handle(OrderStockReservedDomain message, IOrderReadModelStore readModelStore, CancellationToken cancellationToken)
    {
        var current = await readModelStore.GetAsync(message.OrderId, cancellationToken);
        if (current is null || current.Status == "Failed")
        {
            return;
        }

        await readModelStore.UpsertAsync(current with { Status = "StockReserved" }, cancellationToken);
    }

    public static async Task Handle(OrderPaymentAuthorizedDomain message, IOrderReadModelStore readModelStore, CancellationToken cancellationToken)
    {
        var current = await readModelStore.GetAsync(message.OrderId, cancellationToken);
        if (current is null || current.Status == "Failed")
        {
            return;
        }

        await readModelStore.UpsertAsync(
            current with
            {
                Status = "PaymentAuthorized",
                TransactionId = message.TransactionId
            },
            cancellationToken);
    }

    public static async Task Handle(OrderCompletedDomain message, IOrderReadModelStore readModelStore, CancellationToken cancellationToken)
    {
        var current = await readModelStore.GetAsync(message.OrderId, cancellationToken);
        if (current is null || current.Status == "Failed")
        {
            return;
        }

        await readModelStore.UpsertAsync(
            current with
            {
                Status = "Completed",
                TrackingCode = message.TrackingCode,
                TransactionId = message.TransactionId
            },
            cancellationToken);
    }

    public static async Task Handle(OrderFailedDomain message, IOrderReadModelStore readModelStore, CancellationToken cancellationToken)
    {
        var current = await readModelStore.GetAsync(message.OrderId, cancellationToken);
        if (current is null)
        {
            return;
        }

        await readModelStore.UpsertAsync(
            current with
            {
                Status = "Failed",
                FailureReason = message.Reason
            },
            cancellationToken);
    }
}
