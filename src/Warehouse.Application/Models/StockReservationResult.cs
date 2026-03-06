namespace Warehouse.Application.Models;

public sealed record StockReservationResult(Guid OrderId, bool Reserved, string? Reason = null);
