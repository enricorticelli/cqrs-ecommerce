namespace Account.Application.Models;

public sealed record OrderSummary(
    Guid Id,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAtUtc,
    string? TrackingCode,
    string? TransactionId,
    string? FailureReason);
