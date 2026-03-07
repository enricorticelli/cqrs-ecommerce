namespace Order.Api.Contracts;

public sealed record ManualCancelOrderResponse(
    Guid OrderId,
    string Status,
    string Reason,
    string Mode);
