using Order.Application.Models;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Application.Abstractions;

public interface IWarehouseClient
{
    Task<StockReservationDecision> ReserveStockAsync(Guid orderId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken);
}
