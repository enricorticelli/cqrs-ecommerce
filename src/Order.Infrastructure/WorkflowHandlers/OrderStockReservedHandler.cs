using Order.Application;
using Shared.BuildingBlocks.Contracts;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderStockReservedHandler
{
    public Task Handle(StockReservedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandleStockReservedAsync(message, cancellationToken);
    }
}
