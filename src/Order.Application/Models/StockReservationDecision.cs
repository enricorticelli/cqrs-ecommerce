namespace Order.Application.Models;

public sealed record StockReservationDecision(bool Reserved, string? Reason = null);
