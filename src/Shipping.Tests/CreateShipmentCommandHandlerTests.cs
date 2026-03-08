using Moq;
using Shared.BuildingBlocks.Exceptions;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Shipping;
using Shared.BuildingBlocks.Contracts.Messaging;
using Shared.BuildingBlocks.Mapping;
using Microsoft.Extensions.Logging;
using Shipping.Application.Abstractions.Repositories;
using Shipping.Application.Commands;
using Shipping.Application.Services;
using Shipping.Application.Views;
using Xunit;

namespace Shipping.Tests;

public sealed class CreateShipmentCommandHandlerTests
{
    [Fact]
    public async Task Create_should_return_existing_shipment_if_order_already_exists()
    {
        const string customerEmail = "customer@example.com";
        var existing = Shipping.Domain.Entities.Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IShipmentRepository>();
        repository
            .Setup(x => x.GetByOrderIdAsync(existing.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var snapshotRepository = new Mock<IShipmentNotificationSnapshotRepository>();
        var publisher = new Mock<IDomainEventPublisher>();
        var mapper = new Mock<IViewMapper<Shipping.Domain.Entities.Shipment, ShipmentView>>();
        mapper.Setup(x => x.Map(existing)).Returns(ToView(existing));
        var logger = new Mock<ILogger<ShippingCommandService>>();

        var sut = new ShippingCommandService(repository.Object, snapshotRepository.Object, publisher.Object, logger.Object, mapper.Object);
        var result = await sut.CreateAsync(new CreateShipmentCommand(existing.OrderId, existing.UserId, customerEmail), CancellationToken.None);

        Assert.Equal(existing.OrderId, result.OrderId);
        repository.Verify(x => x.Add(It.IsAny<Shipping.Domain.Entities.Shipment>()), Times.Never);
        snapshotRepository.Verify(x => x.UpsertCustomerEmailAsync(existing.OrderId, customerEmail, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_status_should_throw_when_shipment_does_not_exist()
    {
        var repository = new Mock<IShipmentRepository>();
        repository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shipping.Domain.Entities.Shipment?)null);

        var snapshotRepository = new Mock<IShipmentNotificationSnapshotRepository>();
        var publisher = new Mock<IDomainEventPublisher>();
        var mapper = new Mock<IViewMapper<Shipping.Domain.Entities.Shipment, ShipmentView>>();
        var logger = new Mock<ILogger<ShippingCommandService>>();
        var sut = new ShippingCommandService(repository.Object, snapshotRepository.Object, publisher.Object, logger.Object, mapper.Object);

        var action = async () => await sut.UpdateStatusAsync(new UpdateShipmentStatusCommand(Guid.NewGuid(), "InTransit"), CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundAppException>(action);
    }

    [Fact]
    public async Task Update_status_should_publish_intransit_event_on_transition()
    {
        const string customerEmail = "customer@example.com";
        var shipment = Shipping.Domain.Entities.Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IShipmentRepository>();
        repository
            .Setup(x => x.GetByIdAsync(shipment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shipment);

        var snapshotRepository = new Mock<IShipmentNotificationSnapshotRepository>();
        snapshotRepository
            .Setup(x => x.GetCustomerEmailByOrderIdAsync(shipment.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customerEmail);

        var publisher = new Mock<IDomainEventPublisher>();
        publisher
            .Setup(x => x.PublishAndFlushAsync(It.IsAny<Shared.BuildingBlocks.Contracts.IntegrationEvents.IntegrationEventBase>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mapper = new Mock<IViewMapper<Shipping.Domain.Entities.Shipment, ShipmentView>>();
        mapper.Setup(x => x.Map(shipment)).Returns(ToView(shipment));
        var logger = new Mock<ILogger<ShippingCommandService>>();

        var sut = new ShippingCommandService(repository.Object, snapshotRepository.Object, publisher.Object, logger.Object, mapper.Object);

        await sut.UpdateStatusAsync(new UpdateShipmentStatusCommand(shipment.Id, "InTransit"), CancellationToken.None);

        publisher.Verify(
            x => x.PublishAndFlushAsync(
                It.Is<ShipmentInTransitForCommunicationV1>(e => e.OrderId == shipment.OrderId && e.CustomerEmail == customerEmail),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task List_should_map_shipments()
    {
        var shipment = Shipping.Domain.Entities.Shipment.Create(Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IShipmentRepository>();
        repository
            .Setup(x => x.ListAsync(20, 0, "TRK", It.IsAny<CancellationToken>()))
            .ReturnsAsync([shipment]);

        var mapper = new Mock<IViewMapper<Shipping.Domain.Entities.Shipment, ShipmentView>>();
        mapper.Setup(x => x.Map(shipment)).Returns(ToView(shipment));

        var sut = new ShippingQueryService(repository.Object, mapper.Object);
        var result = await sut.ListAsync(20, 0, "TRK", CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(shipment.Id, result[0].Id);
    }

    private static ShipmentView ToView(Shipping.Domain.Entities.Shipment shipment)
    {
        return new ShipmentView(
            shipment.Id,
            shipment.OrderId,
            shipment.UserId,
            shipment.TrackingCode,
            shipment.Status,
            shipment.CreatedAtUtc,
            shipment.UpdatedAtUtc,
            shipment.DeliveredAtUtc);
    }
}
