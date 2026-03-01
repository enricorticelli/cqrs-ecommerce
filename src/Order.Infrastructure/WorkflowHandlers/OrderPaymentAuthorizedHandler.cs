using Order.Application;
using Shared.BuildingBlocks.Contracts;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderPaymentAuthorizedHandler
{
    public Task Handle(PaymentAuthorizedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandlePaymentAuthorizedAsync(message, cancellationToken);
    }
}
