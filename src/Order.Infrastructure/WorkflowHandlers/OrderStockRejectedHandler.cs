using Order.Application;
using Order.Application.Abstractions;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderStockRejectedHandler
{
    public Task Handle(StockRejectedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandleStockRejectedAsync(message, cancellationToken);
    }
}
