namespace Shared.BuildingBlocks.Contracts;

public sealed record OrderItemDto(Guid ProductId, string Sku, string Name, int Quantity, decimal UnitPrice);
