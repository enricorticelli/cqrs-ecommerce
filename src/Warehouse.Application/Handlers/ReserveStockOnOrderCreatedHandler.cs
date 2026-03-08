using Shared.BuildingBlocks.Contracts.IntegrationEvents;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Warehouse;
using Shared.BuildingBlocks.Contracts.Messaging;
using Warehouse.Application.Abstractions.Idempotency;
using Warehouse.Application.Abstractions.Services;
using Warehouse.Application.Services;

namespace Warehouse.Application.Handlers;

public sealed class ReserveStockOnOrderCreatedHandler(
    IStockReservationService stockReservationService,
    IWarehouseEventDeduplicationStore deduplicationStore,
    IDomainEventPublisher eventPublisher)
{
    public async Task HandleAsync(OrderCreatedV1 orderCreatedEvent, CancellationToken cancellationToken)
    {
        if (await deduplicationStore.HasProcessedAsync(orderCreatedEvent.Metadata.EventId, cancellationToken))
        {
            return;
        }

        var result = await stockReservationService.ReserveAsync(orderCreatedEvent, cancellationToken);

        IntegrationEventBase integrationEvent = result.IsReserved
            ? new StockReservedV1(result.OrderId, CreateMetadata(orderCreatedEvent.Metadata.CorrelationId))
            : new StockRejectedV1(
                result.OrderId,
                result.FailureReason ?? "Stock reservation failed.",
                CreateMetadata(orderCreatedEvent.Metadata.CorrelationId));

        await eventPublisher.PublishAndFlushAsync(integrationEvent, cancellationToken);
        await deduplicationStore.MarkProcessedAsync(orderCreatedEvent.Metadata.EventId, cancellationToken);
    }

    private static IntegrationEventMetadata CreateMetadata(string correlationId)
    {
        return new IntegrationEventMetadata(Guid.NewGuid(), DateTimeOffset.UtcNow, correlationId, "Warehouse");
    }
}
