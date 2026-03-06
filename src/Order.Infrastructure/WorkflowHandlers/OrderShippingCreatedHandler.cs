using Order.Application;
using Order.Application.Abstractions;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderShippingCreatedHandler
{
    public Task Handle(ShippingCreatedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandleShippingCreatedAsync(message, cancellationToken);
    }
}
