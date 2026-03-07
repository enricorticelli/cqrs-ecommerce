using Shipping.Api.Contracts;
using Shipping.Api.Contracts.Requests;
using Shipping.Api.Contracts.Responses;
using Shared.BuildingBlocks.Api;

namespace Shipping.Api.Endpoints;

public static class ShippingEndpoints
{
    public static RouteGroupBuilder MapShippingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ShippingRoutes.Base)
            .WithTags("Shipping");

        group.MapPost("/", CreateShipment)
            .WithName("CreateShipment");
        group.MapGet("/", ListShipments)
            .WithName("ListShipments");
        group.MapGet("/orders/{orderId:guid}", GetShipmentByOrder)
            .WithName("GetShipmentByOrder");
        group.MapPost("/{shipmentId:guid}/status", UpdateShipmentStatus)
            .WithName("UpdateShipmentStatus");
        return group;
    }

    private static IResult CreateShipment(CreateShipmentRequest request)
    {
        var trackingCode = $"TRK-{Guid.NewGuid():N}"[..16];
        var response = new CreateShipmentResponse(request.OrderId, trackingCode);
        return Results.Created($"{ShippingRoutes.Base}/orders/{request.OrderId}", response);
    }

    private static IResult ListShipments()
    {
        return Results.Ok(new[] { BuildShipment(Guid.NewGuid(), Guid.NewGuid(), "InPreparation") });
    }

    private static IResult GetShipmentByOrder(Guid orderId)
    {
        return Results.Ok(BuildShipment(Guid.NewGuid(), orderId, "InPreparation"));
    }

    private static IResult UpdateShipmentStatus(Guid shipmentId, UpdateShipmentStatusRequest request)
    {
        return Results.Ok(BuildShipment(shipmentId, Guid.NewGuid(), request.Status));
    }

    private static ShipmentResponse BuildShipment(Guid shipmentId, Guid orderId, string status)
    {
        var now = DateTimeOffset.UtcNow;
        return new ShipmentResponse(
            shipmentId,
            orderId,
            Guid.NewGuid(),
            $"TRK-{shipmentId:N}"[..16],
            status,
            now.AddMinutes(-30),
            now,
            status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ? now : null);
    }
}
