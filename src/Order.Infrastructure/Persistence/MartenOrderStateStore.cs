using Marten;
using Order.Application;
using Order.Domain;
using Shared.BuildingBlocks.Contracts;

namespace Order.Infrastructure.Persistence;

public sealed class MartenOrderStateStore(IDocumentSession documentSession, IQuerySession querySession) : IOrderStateStore
{
    public async Task StartOrderAsync(Guid orderId, Guid cartId, Guid userId, IReadOnlyList<OrderItemDto> items, decimal totalAmount, CancellationToken cancellationToken)
    {
        documentSession.Events.StartStream<OrderAggregate>(orderId, new OrderPlacedDomain(orderId, cartId, userId, items, totalAmount));
        await documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderAggregateState?> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await querySession.Events.AggregateStreamAsync<OrderAggregate>(orderId, token: cancellationToken);
        return order is null
            ? null
            : new OrderAggregateState(
                order.Id,
                order.CartId,
                order.UserId,
                order.Status,
                order.TotalAmount,
                order.Items,
                order.TransactionId,
                order.TrackingCode,
                order.FailureReason);
    }

    public async Task<OrderView?> GetOrderViewAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await querySession.Events.AggregateStreamAsync<OrderAggregate>(orderId, token: cancellationToken);
        return order is null
            ? null
            : new OrderView(
                order.Id,
                order.CartId,
                order.UserId,
                order.Status.ToString(),
                order.TotalAmount,
                order.Items,
                order.TrackingCode,
                order.TransactionId,
                order.FailureReason);
    }

    public async Task MarkStockReservedAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        stream.AppendOne(new OrderStockReservedDomain(orderId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkPaymentAuthorizedAsync(Guid orderId, string transactionId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        stream.AppendOne(new OrderPaymentAuthorizedDomain(orderId, transactionId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkCompletedAsync(Guid orderId, string trackingCode, string transactionId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        stream.AppendOne(new OrderCompletedDomain(orderId, trackingCode, transactionId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        stream.AppendOne(new OrderFailedDomain(orderId, reason));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
