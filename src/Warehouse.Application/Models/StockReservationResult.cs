namespace Warehouse.Application;

public sealed record StockReservationResult(Guid OrderId, bool Reserved, string? Reason = null);
