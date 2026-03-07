using Payment.Domain.Events;
using Payment.Infrastructure.Persistence.ReadModels;

namespace Payment.Infrastructure.Messaging.Handlers;

public sealed class PaymentDomainEventProjectionHandler
{
    public static Task Handle(
        PaymentSessionCreatedDomain message,
        IPaymentReadModelStore readModelStore,
        CancellationToken cancellationToken)
    {
        var row = new PaymentReadModelRow(
            message.SessionId,
            message.OrderId,
            message.UserId,
            message.Amount,
            message.PaymentMethod,
            "Pending",
            null,
            null,
            message.CreatedAtUtc,
            null);
        return readModelStore.UpsertAsync(row, cancellationToken);
    }

    public static async Task Handle(
        PaymentSessionAuthorizedDomain message,
        IPaymentReadModelStore readModelStore,
        CancellationToken cancellationToken)
    {
        var current = await readModelStore.GetBySessionIdAsync(message.SessionId, cancellationToken);
        if (current is null || current.Status != "Pending")
        {
            return;
        }

        await readModelStore.UpsertAsync(
            current with
            {
                Status = "Authorized",
                TransactionId = message.TransactionId,
                FailureReason = null,
                CompletedAtUtc = message.AuthorizedAtUtc
            },
            cancellationToken);
    }

    public static async Task Handle(
        PaymentSessionRejectedDomain message,
        IPaymentReadModelStore readModelStore,
        CancellationToken cancellationToken)
    {
        var current = await readModelStore.GetBySessionIdAsync(message.SessionId, cancellationToken);
        if (current is null || current.Status != "Pending")
        {
            return;
        }

        await readModelStore.UpsertAsync(
            current with
            {
                Status = "Rejected",
                FailureReason = message.Reason,
                TransactionId = null,
                CompletedAtUtc = message.RejectedAtUtc
            },
            cancellationToken);
    }
}
