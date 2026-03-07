using Order.Application.Abstractions;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.WorkflowHandlers;

public sealed class OrderPaymentAuthorizedHandler
{
    public Task Handle(PaymentAuthorizedV1 message, IOrderWorkflowProcessor processor, CancellationToken cancellationToken)
    {
        return processor.HandlePaymentAuthorizedAsync(message, cancellationToken);
    }
}
