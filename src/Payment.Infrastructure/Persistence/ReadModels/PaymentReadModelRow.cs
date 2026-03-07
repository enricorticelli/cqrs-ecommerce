namespace Payment.Infrastructure.Persistence.ReadModels;

public sealed record PaymentReadModelRow(
    Guid SessionId,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string PaymentMethod,
    string Status,
    string? TransactionId,
    string? FailureReason,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? CompletedAtUtc);
