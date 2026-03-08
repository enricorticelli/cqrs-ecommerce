using Communication.Application.Abstractions.Email;
using Communication.Application.Abstractions.Idempotency;
using Microsoft.Extensions.Logging;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;
using Shared.BuildingBlocks.Messaging;

namespace Communication.Application.Handlers;

public sealed class SendOrderCompletedEmailHandler(
    IEmailSender emailSender,
    ICommunicationEventDeduplicationStore deduplicationStore,
    ILogger<SendOrderCompletedEmailHandler> logger)
    : IntegrationEventHandlerBase<OrderCompletedForCommunicationV1>(deduplicationStore, logger)
{
    public Task Handle(OrderCompletedForCommunicationV1 integrationEvent, CancellationToken cancellationToken)
    {
        return HandleDeduplicatedAsync(
            integrationEvent,
            ct => emailSender.SendAsync(
                integrationEvent.CustomerEmail,
                $"Conferma ordine {integrationEvent.OrderId}",
                $"Il tuo ordine {integrationEvent.OrderId} e' stato confermato. Totale: {integrationEvent.TotalAmount:0.00}.",
                ct),
            cancellationToken);
    }
}
