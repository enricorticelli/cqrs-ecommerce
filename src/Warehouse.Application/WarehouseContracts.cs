using Shared.BuildingBlocks.Contracts;

namespace Warehouse.Application;

public sealed record StockReservationResult(Guid OrderId, bool Reserved, string? Reason = null);

public interface IWarehouseService
{
    Task<StockReservationResult> ReserveStockAsync(StockReserveRequestedV1 request, CancellationToken cancellationToken);
    Task<int> SeedStockAsync(CancellationToken cancellationToken);
}
