using Shared.BuildingBlocks.Contracts;

namespace Order.Application;

public interface IWarehouseClient
{
    Task<StockReservationDecision> ReserveStockAsync(Guid orderId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken);
}
