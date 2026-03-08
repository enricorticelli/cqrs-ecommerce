using Shared.BuildingBlocks.Contracts.IntegrationEvents;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Shipping;
using Shared.BuildingBlocks.Contracts.Messaging;
using Shared.BuildingBlocks.Exceptions;
using Shared.BuildingBlocks.Mapping;
using Microsoft.Extensions.Logging;
using Shipping.Application.Abstractions.Commands;
using Shipping.Application.Abstractions.Repositories;
using Shipping.Application.Commands;
using Shipping.Application.Views;

namespace Shipping.Application.Services;

public sealed class ShippingCommandService(
    IShipmentRepository shipmentRepository,
    IShipmentNotificationSnapshotRepository shipmentNotificationSnapshotRepository,
    IDomainEventPublisher eventPublisher,
    ILogger<ShippingCommandService> logger,
    IViewMapper<Shipping.Domain.Entities.Shipment, ShipmentView> mapper) : IShippingCommandService
{
    public async Task<ShipmentView> CreateAsync(CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        await shipmentNotificationSnapshotRepository.UpsertCustomerEmailAsync(command.OrderId, command.CustomerEmail, cancellationToken);

        var existing = await shipmentRepository.GetByOrderIdAsync(command.OrderId, cancellationToken);
        if (existing is not null)
        {
            await shipmentRepository.SaveChangesAsync(cancellationToken);
            return mapper.Map(existing);
        }

        var shipment = Shipping.Domain.Entities.Shipment.Create(command.OrderId, command.UserId);
        shipmentRepository.Add(shipment);
        await shipmentRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map(shipment);
    }

    public async Task<ShipmentView> UpdateStatusAsync(UpdateShipmentStatusCommand command, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(command.ShipmentId, cancellationToken)
            ?? throw new NotFoundAppException($"Shipment '{command.ShipmentId}' not found.");

        var previousStatus = shipment.Status;
        shipment.UpdateStatus(command.Status.Trim());

        if (!string.Equals(previousStatus, shipment.Status, StringComparison.OrdinalIgnoreCase)
            && string.Equals(shipment.Status, Shipping.Domain.Entities.ShipmentStatus.InTransit, StringComparison.OrdinalIgnoreCase))
        {
            var customerEmail = await shipmentNotificationSnapshotRepository
                .GetCustomerEmailByOrderIdAsync(shipment.OrderId, cancellationToken);

            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                logger.LogWarning(
                    "Skipping ShipmentInTransit notification due to missing customer email snapshot. shipmentId={ShipmentId} orderId={OrderId}",
                    shipment.Id,
                    shipment.OrderId);

                await shipmentRepository.SaveChangesAsync(cancellationToken);
                return mapper.Map(shipment);
            }

            var integrationEvent = new ShipmentInTransitForCommunicationV1(
                shipment.OrderId,
                shipment.TrackingCode,
                customerEmail,
                CreateMetadata($"shipping-{shipment.Id}", "Shipping"));

            await eventPublisher.PublishAndFlushAsync(integrationEvent, cancellationToken);
        }

        await shipmentRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map(shipment);
    }

    private static IntegrationEventMetadata CreateMetadata(string correlationId, string sourceContext)
    {
        return IntegrationEventMetadataFactory.Create(correlationId, sourceContext);
    }
}
