using Shared.BuildingBlocks.Contracts.Integration;
using Shipping.Api.Contracts.Requests;
using Shipping.Api.Contracts.Responses;
using Shipping.Application.Commands;
using Shipping.Application.Models;

namespace Shipping.Api.Mappers;

public static class ShippingMapper
{
    public static CreateShipmentCommand ToCreateShipmentCommand(CreateShipmentRequest request)
    {
        var payload = new ShippingCreateRequestedV1(
            request.OrderId,
            request.UserId,
            request.Items.Select(item => new OrderItemDto(item.ProductId, item.Sku, item.Name, item.Quantity, item.UnitPrice)).ToArray());

        return new CreateShipmentCommand(payload);
    }

    public static CreateShipmentResponse ToCreateShipmentResponse(Guid orderId, string trackingCode)
        => new(orderId, trackingCode);

    public static UpdateShipmentStatusCommand ToUpdateShipmentStatusCommand(Guid shipmentId, UpdateShipmentStatusRequest request)
        => new(shipmentId, request.Status);

    public static ShipmentResponse ToResponse(ShipmentView view)
        => new(
            view.Id,
            view.OrderId,
            view.UserId,
            view.TrackingCode,
            view.Status,
            view.CreatedAtUtc,
            view.UpdatedAtUtc,
            view.DeliveredAtUtc);
}
