namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record OrderItemDto(Guid ProductId, string Sku, string Name, int Quantity, decimal UnitPrice);
