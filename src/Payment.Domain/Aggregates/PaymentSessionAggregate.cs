using Payment.Domain.Enums;
using Payment.Domain.Events;

namespace Payment.Domain.Aggregates;

public sealed class PaymentSessionAggregate
{
    public Guid SessionId { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string PaymentMethod { get; private set; } = string.Empty;
    public PaymentSessionStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public PaymentSessionAggregate()
    {
    }

    public bool CanAuthorize()
    {
        return Status == PaymentSessionStatus.Pending;
    }

    public bool CanReject()
    {
        return Status == PaymentSessionStatus.Pending;
    }

    public void Apply(PaymentSessionCreatedDomain @event)
    {
        SessionId = @event.SessionId;
        OrderId = @event.OrderId;
        UserId = @event.UserId;
        Amount = @event.Amount;
        PaymentMethod = @event.PaymentMethod;
        CreatedAtUtc = @event.CreatedAtUtc;
        Status = PaymentSessionStatus.Pending;
    }

    public void Apply(PaymentSessionAuthorizedDomain @event)
    {
        Status = PaymentSessionStatus.Authorized;
        TransactionId = @event.TransactionId;
        CompletedAtUtc = @event.AuthorizedAtUtc;
        FailureReason = null;
    }

    public void Apply(PaymentSessionRejectedDomain @event)
    {
        Status = PaymentSessionStatus.Rejected;
        FailureReason = @event.Reason;
        CompletedAtUtc = @event.RejectedAtUtc;
        TransactionId = null;
    }
}
