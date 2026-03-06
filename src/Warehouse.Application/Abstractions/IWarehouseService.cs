using Shared.BuildingBlocks.Contracts.Integration;
using Warehouse.Application.Models;

namespace Warehouse.Application.Abstractions;

public interface IWarehouseService
{
    Task<StockReservationResult> ReserveStockAsync(StockReserveRequestedV1 request, CancellationToken cancellationToken);
    Task UpsertStockAsync(UpsertStockItem model, CancellationToken cancellationToken);
}
