namespace Order.Api.Contracts.Responses;

public sealed record ManualCompleteOrderResponse(
    Guid OrderId,
    string Status,
    string TrackingCode,
    string TransactionId,
    string Mode);
