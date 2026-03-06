namespace Payment.Domain.Events;

public sealed record PaymentSessionRejectedDomain(Guid SessionId, string Reason, DateTimeOffset RejectedAtUtc);
