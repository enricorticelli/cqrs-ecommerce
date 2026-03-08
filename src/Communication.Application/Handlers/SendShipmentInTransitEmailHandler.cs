using Communication.Application.Abstractions.Email;
using Communication.Application.Abstractions.Idempotency;
using Microsoft.Extensions.Logging;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Shipping;
using Shared.BuildingBlocks.Messaging;

namespace Communication.Application.Handlers;

public sealed class SendShipmentInTransitEmailHandler(
    IEmailSender emailSender,
    ICommunicationEventDeduplicationStore deduplicationStore,
    ILogger<SendShipmentInTransitEmailHandler> logger)
    : IntegrationEventHandlerBase<ShipmentInTransitForCommunicationV1>(deduplicationStore, logger)
{
    public Task Handle(ShipmentInTransitForCommunicationV1 integrationEvent, CancellationToken cancellationToken)
    {
        return HandleDeduplicatedAsync(
            integrationEvent,
            ct => emailSender.SendAsync(
                integrationEvent.CustomerEmail,
                $"Ordine {integrationEvent.OrderId} spedito",
                $"Il tuo ordine {integrationEvent.OrderId} e' stato spedito. Tracking: {integrationEvent.TrackingCode}.",
                ct),
            cancellationToken);
    }
}
