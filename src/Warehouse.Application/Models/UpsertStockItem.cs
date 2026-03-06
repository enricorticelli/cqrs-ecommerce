namespace Warehouse.Application.Models;

public sealed record UpsertStockItem(Guid ProductId, string Sku, int AvailableQuantity);
