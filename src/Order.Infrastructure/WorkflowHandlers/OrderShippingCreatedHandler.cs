using Order.Application;
using Shared.BuildingBlocks.Contracts;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderShippingCreatedHandler
{
    public Task Handle(ShippingCreatedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandleShippingCreatedAsync(message, cancellationToken);
    }
}
