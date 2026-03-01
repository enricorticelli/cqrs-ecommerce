namespace Order.Application;

public sealed record StockReservationDecision(bool Reserved, string? Reason = null);
