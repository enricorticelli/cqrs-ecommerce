using Marten;
using Payment.Application.Abstractions;
using Payment.Domain.Aggregates;
using Payment.Domain.Events;
using Shared.BuildingBlocks.Contracts.Integration;
using Wolverine;

namespace Payment.Infrastructure.Persistence;

public sealed class MartenPaymentStateStore(
    IDocumentSession documentSession,
    IMessageBus bus) : IPaymentStateStore
{
    public async Task<Guid> StartSessionAsync(Guid orderId, Guid userId, decimal amount, string paymentMethod, CancellationToken cancellationToken)
    {
        var sessionId = Guid.NewGuid();
        var created = new PaymentSessionCreatedDomain(
            sessionId,
            orderId,
            userId,
            amount,
            paymentMethod,
            DateTimeOffset.UtcNow);

        documentSession.Events.StartStream<PaymentSessionAggregate>(sessionId, created);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(created);
        return sessionId;
    }

    public async Task<bool> AuthorizeSessionAsync(Guid sessionId, string transactionId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<PaymentSessionAggregate>(sessionId, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate is null || !stream.Aggregate.CanAuthorize())
        {
            return false;
        }

        var authorized = new PaymentSessionAuthorizedDomain(sessionId, transactionId, DateTimeOffset.UtcNow);
        stream.AppendOne(authorized);
        await documentSession.SaveChangesAsync(cancellationToken);

        await bus.PublishAsync(authorized);
        await bus.PublishAsync(new PaymentAuthorizedV1(stream.Aggregate.OrderId, transactionId));
        return true;
    }

    public async Task<bool> RejectSessionAsync(Guid sessionId, string reason, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<PaymentSessionAggregate>(sessionId, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate is null || !stream.Aggregate.CanReject())
        {
            return false;
        }

        var rejected = new PaymentSessionRejectedDomain(sessionId, reason, DateTimeOffset.UtcNow);
        stream.AppendOne(rejected);
        await documentSession.SaveChangesAsync(cancellationToken);

        await bus.PublishAsync(rejected);
        await bus.PublishAsync(new PaymentFailedV1(stream.Aggregate.OrderId, reason));
        return true;
    }
}
