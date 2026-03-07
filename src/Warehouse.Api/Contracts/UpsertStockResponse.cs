namespace Warehouse.Api.Contracts;

public sealed record UpsertStockResponse(Guid ProductId, string Sku, int AvailableQuantity);
