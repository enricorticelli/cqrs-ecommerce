using Microsoft.Extensions.Logging;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;
using Shared.BuildingBlocks.Messaging;
using Shipping.Application.Abstractions.Commands;
using Shipping.Application.Abstractions.Idempotency;
using Shipping.Application.Commands;

namespace Shipping.Application.Handlers;

public sealed class CreateShipmentOnOrderCompletedHandler(
    IShippingCommandService shippingCommandService,
    IShippingEventDeduplicationStore deduplicationStore,
    ILogger<CreateShipmentOnOrderCompletedHandler> logger)
    : IntegrationEventHandlerBase<OrderCompletedForCommunicationV1>(deduplicationStore, logger)
{
    public Task Handle(OrderCompletedForCommunicationV1 integrationEvent, CancellationToken cancellationToken)
    {
        return HandleDeduplicatedAsync(
            integrationEvent,
            ct => shippingCommandService.CreateAsync(
                new CreateShipmentCommand(integrationEvent.OrderId, integrationEvent.UserId, integrationEvent.CustomerEmail),
                ct),
            cancellationToken);
    }
}
