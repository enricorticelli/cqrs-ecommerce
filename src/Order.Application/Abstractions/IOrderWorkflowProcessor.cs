using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Application.Abstractions;

public interface IOrderWorkflowProcessor
{
    Task HandleStockReservedAsync(StockReservedV1 message, CancellationToken cancellationToken);
    Task HandleStockRejectedAsync(StockRejectedV1 message, CancellationToken cancellationToken);
    Task HandlePaymentAuthorizedAsync(PaymentAuthorizedV1 message, CancellationToken cancellationToken);
    Task HandlePaymentFailedAsync(PaymentFailedV1 message, CancellationToken cancellationToken);
    Task HandleShippingCreatedAsync(ShippingCreatedV1 message, CancellationToken cancellationToken);
}
