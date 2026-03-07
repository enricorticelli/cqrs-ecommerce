using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Domain.Enums;
using Order.Infrastructure.Persistence.ReadModels;

namespace Order.Infrastructure;

public sealed class OrderStateReader(IOrderReadModelStore orderReadModelStore) : IOrderStateReader
{
    public async Task<OrderAggregateState?> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var readModel = await orderReadModelStore.GetAsync(orderId, cancellationToken);
        if (readModel is null)
        {
            return null;
        }

        var status = Enum.TryParse<OrderStatus>(readModel.Status, out var parsed)
            ? parsed
            : OrderStatus.Pending;

        return new OrderAggregateState(
            readModel.OrderId,
            readModel.CartId,
            readModel.UserId,
            readModel.IdentityType,
            readModel.PaymentMethod,
            readModel.AuthenticatedUserId,
            readModel.AnonymousId,
            readModel.Customer,
            readModel.ShippingAddress,
            readModel.BillingAddress,
            status,
            readModel.TotalAmount,
            readModel.Items,
            readModel.TransactionId,
            readModel.TrackingCode,
            readModel.FailureReason);
    }
}
