namespace Payment.Domain.Events;

public sealed record PaymentSessionCreatedDomain(
    Guid SessionId,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string PaymentMethod,
    DateTimeOffset CreatedAtUtc);
