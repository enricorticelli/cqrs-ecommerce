using Order.Application.Abstractions;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderPaymentFailedHandler
{
    public Task Handle(PaymentFailedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandlePaymentFailedAsync(message, cancellationToken);
    }
}
