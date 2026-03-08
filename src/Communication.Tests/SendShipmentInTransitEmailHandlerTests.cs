using Communication.Application.Abstractions.Email;
using Communication.Application.Abstractions.Idempotency;
using Communication.Application.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.BuildingBlocks.Contracts.IntegrationEvents;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Shipping;
using Xunit;

namespace Communication.Tests;

public sealed class SendShipmentInTransitEmailHandlerTests
{
    [Fact]
    public async Task Should_send_email_and_mark_event_processed_once()
    {
        var integrationEvent = new ShipmentInTransitForCommunicationV1(
            Guid.NewGuid(),
            "TRK-123",
            "customer@example.com",
            new IntegrationEventMetadata(Guid.NewGuid(), DateTimeOffset.UtcNow, "corr-1", "Shipping"));

        var emailSender = new Mock<IEmailSender>();
        var deduplicationStore = new Mock<ICommunicationEventDeduplicationStore>();
        deduplicationStore.Setup(x => x.HasProcessedAsync(integrationEvent.Metadata.EventId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var logger = new Mock<ILogger<SendShipmentInTransitEmailHandler>>();
        var sut = new SendShipmentInTransitEmailHandler(emailSender.Object, deduplicationStore.Object, logger.Object);

        await sut.Handle(integrationEvent, CancellationToken.None);

        emailSender.Verify(x => x.SendAsync(integrationEvent.CustomerEmail, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        deduplicationStore.Verify(x => x.MarkProcessedAsync(integrationEvent.Metadata.EventId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
