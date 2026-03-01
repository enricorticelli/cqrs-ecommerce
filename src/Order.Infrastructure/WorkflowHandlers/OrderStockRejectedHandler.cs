using Order.Application;
using Shared.BuildingBlocks.Contracts;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderStockRejectedHandler
{
    public Task Handle(StockRejectedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandleStockRejectedAsync(message, cancellationToken);
    }
}
