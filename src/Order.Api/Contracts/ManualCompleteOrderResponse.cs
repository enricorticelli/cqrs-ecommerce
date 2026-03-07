namespace Order.Api.Contracts;

public sealed record ManualCompleteOrderResponse(
    Guid OrderId,
    string Status,
    string TrackingCode,
    string TransactionId,
    string Mode);
