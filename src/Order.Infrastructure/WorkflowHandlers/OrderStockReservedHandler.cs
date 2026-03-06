using Order.Application;
using Order.Application.Abstractions;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderStockReservedHandler
{
    public Task Handle(StockReservedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandleStockReservedAsync(message, cancellationToken);
    }
}
