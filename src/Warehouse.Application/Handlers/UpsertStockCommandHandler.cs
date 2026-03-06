using Shared.BuildingBlocks.Cqrs.Abstractions;
using Warehouse.Application.Abstractions;
using Warehouse.Application.Commands;

namespace Warehouse.Application.Handlers;

public sealed class UpsertStockCommandHandler(IWarehouseService warehouseService)
    : ICommandHandler<UpsertStockCommand, Unit>
{
    public async Task<Unit> HandleAsync(UpsertStockCommand command, CancellationToken cancellationToken)
    {
        await warehouseService.UpsertStockAsync(command.Item, cancellationToken);
        return Unit.Value;
    }
}
