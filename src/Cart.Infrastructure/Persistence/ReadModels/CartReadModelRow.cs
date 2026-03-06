using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Cart.Infrastructure.Persistence.ReadModels;

public sealed record CartReadModelRow(Guid CartId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);
