namespace Order.Application.Models;

public sealed record ManualOrderActionResult(
    ManualOrderActionOutcome Outcome,
    string Status,
    string? TrackingCode,
    string? TransactionId,
    string? Reason,
    string? Detail)
{
    public static ManualOrderActionResult NotFound() => new(
        ManualOrderActionOutcome.NotFound,
        string.Empty,
        null,
        null,
        null,
        null);

    public static ManualOrderActionResult Conflict(string detail) => new(
        ManualOrderActionOutcome.Conflict,
        string.Empty,
        null,
        null,
        null,
        detail);
}
