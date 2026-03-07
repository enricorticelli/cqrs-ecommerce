namespace Warehouse.Api.Contracts;

public sealed record ReserveStockResponse(Guid OrderId, bool Reserved, string? Reason);
