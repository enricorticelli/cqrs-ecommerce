namespace Payment.Domain.Events;

public sealed record PaymentSessionAuthorizedDomain(Guid SessionId, string TransactionId, DateTimeOffset AuthorizedAtUtc);
