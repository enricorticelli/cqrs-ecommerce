using Order.Application;
using Order.Application.Abstractions;
using Order.Application.Views;
using Order.Infrastructure.Persistence.ReadModels;

namespace Order.Infrastructure;

public sealed class OrderReadService(IOrderReadModelStore orderReadModelStore) : IOrderQueryService
{
    public async Task<OrderView?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var readModel = await orderReadModelStore.GetAsync(orderId, cancellationToken);
        return readModel is null
            ? null
            : new OrderView(
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
                readModel.Status,
                readModel.TotalAmount,
                readModel.Items,
                readModel.TrackingCode,
                readModel.TransactionId,
                readModel.FailureReason);
    }

    public async Task<IReadOnlyList<OrderView>> GetOrdersAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        var rows = await orderReadModelStore.ListAsync(limit, offset, cancellationToken);
        return rows
            .Select(readModel => new OrderView(
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
                readModel.Status,
                readModel.TotalAmount,
                readModel.Items,
                readModel.TrackingCode,
                readModel.TransactionId,
                readModel.FailureReason))
            .ToList();
    }
}
