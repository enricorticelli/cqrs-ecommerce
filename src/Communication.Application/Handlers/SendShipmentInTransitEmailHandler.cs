using Communication.Application.Abstractions.Email;
using Communication.Application.Abstractions.Idempotency;
using Communication.Domain.Entities;
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
        var message = CommunicationEmailMessage.ForShipmentInTransit(
            integrationEvent.OrderId,
            integrationEvent.TrackingCode,
            integrationEvent.CustomerEmail);

        return HandleDeduplicatedAsync(
            integrationEvent,
            ct => emailSender.SendAsync(
                message.Recipient,
                message.Subject,
                message.Body,
                ct),
            cancellationToken);
    }
}
