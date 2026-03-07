using Microsoft.AspNetCore.Http.HttpResults;
using Shipping.Api.Contracts;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application;
using Shipping.Application.Commands;
using Shipping.Application.Models;
using Shipping.Application.Queries;

namespace Shipping.Api.Endpoints;

public static class ShippingEndpoints
{
    public static RouteGroupBuilder MapShippingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ShippingRoutes.Base)
            .WithTags("Shipping")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

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

    private static async Task<Ok<CreateShipmentResponse>> CreateShipment(
        ShippingCreateRequestedV1 request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.ExecuteAsync(new CreateShipmentCommand(request), cancellationToken);
        return TypedResults.Ok(new CreateShipmentResponse(result.OrderId, result.TrackingCode));
    }

    private static async Task<Ok<IReadOnlyList<ShipmentView>>> ListShipments(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var items = await queryDispatcher.ExecuteAsync(
            new ListShipmentsQuery(limit ?? 50, offset ?? 0),
            cancellationToken);
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Ok<ShipmentView>, NotFound>> GetShipmentByOrder(
        Guid orderId,
        IQueryDispatcher queryDispatcher,
        CancellationToken cancellationToken)
    {
        var shipment = await queryDispatcher.ExecuteAsync(new GetShipmentByOrderIdQuery(orderId), cancellationToken);
        return shipment is null ? TypedResults.NotFound() : TypedResults.Ok(shipment);
    }

    private static async Task<Results<Ok<ShipmentView>, NotFound>> UpdateShipmentStatus(
        Guid shipmentId,
        UpdateShipmentStatusRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var updated = await commandDispatcher.ExecuteAsync(
            new UpdateShipmentStatusCommand(shipmentId, request.Status),
            cancellationToken);
        return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
    }
}
