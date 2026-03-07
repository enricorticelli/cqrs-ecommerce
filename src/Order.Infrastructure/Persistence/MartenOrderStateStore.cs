using Marten;
using Order.Application.Abstractions;
using Order.Domain.Aggregates;
using Order.Domain.Events;
using Shared.BuildingBlocks.Contracts.Integration;
using Wolverine;

namespace Order.Infrastructure.Persistence;

public sealed class MartenOrderStateStore(
    IDocumentSession documentSession,
    IMessageBus bus) : IOrderStateStore
{
    public async Task StartOrderAsync(
        Guid orderId,
        Guid cartId,
        Guid userId,
        string identityType,
        string paymentMethod,
        Guid? authenticatedUserId,
        Guid? anonymousId,
        OrderCustomerDetails customer,
        OrderAddress shippingAddress,
        OrderAddress billingAddress,
        IReadOnlyList<OrderItemDto> items,
        decimal totalAmount,
        CancellationToken cancellationToken)
    {
        var placedEvent = new OrderPlacedDomain(
            orderId,
            cartId,
            userId,
            items,
            totalAmount,
            paymentMethod,
            identityType,
            authenticatedUserId,
            anonymousId,
            customer,
            shippingAddress,
            billingAddress);
        documentSession.Events.StartStream<OrderAggregate>(orderId, placedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(placedEvent);
    }

    public async Task MarkStockReservedAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        var reservedEvent = new OrderStockReservedDomain(orderId);
        stream.AppendOne(reservedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(reservedEvent);
    }

    public async Task MarkPaymentAuthorizedAsync(Guid orderId, string transactionId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        var authorizedEvent = new OrderPaymentAuthorizedDomain(orderId, transactionId);
        stream.AppendOne(authorizedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(authorizedEvent);
    }

    public async Task MarkCompletedAsync(Guid orderId, string trackingCode, string transactionId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        var completedEvent = new OrderCompletedDomain(orderId, trackingCode, transactionId);
        stream.AppendOne(completedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(completedEvent);
    }

    public async Task MarkFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<OrderAggregate>(orderId, cancellationToken);
        var failedEvent = new OrderFailedDomain(orderId, reason);
        stream.AppendOne(failedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(failedEvent);
    }

}
