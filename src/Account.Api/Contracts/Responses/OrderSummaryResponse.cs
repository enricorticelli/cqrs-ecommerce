namespace Account.Api.Contracts.Responses;

public sealed record OrderSummaryResponse(
    Guid Id,
    string Status,
    decimal TotalAmount,
    string CreatedAtUtc,
    string? TrackingCode,
    string? TransactionId,
    string? FailureReason);
