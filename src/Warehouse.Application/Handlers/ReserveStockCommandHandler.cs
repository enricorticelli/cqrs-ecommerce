using Shared.BuildingBlocks.Cqrs.Abstractions;
using Warehouse.Application.Abstractions;
using Warehouse.Application.Commands;
using Warehouse.Application.Models;

namespace Warehouse.Application.Handlers;

public sealed class ReserveStockCommandHandler(IWarehouseService warehouseService)
    : ICommandHandler<ReserveStockCommand, StockReservationResult>
{
    public Task<StockReservationResult> HandleAsync(ReserveStockCommand command, CancellationToken cancellationToken)
    {
        return warehouseService.ReserveStockAsync(command.Request, cancellationToken);
    }
}
