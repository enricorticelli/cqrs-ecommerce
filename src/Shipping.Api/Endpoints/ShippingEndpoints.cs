using Microsoft.AspNetCore.Http.HttpResults;
using Shipping.Api.Contracts;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Api.Contracts.Requests;
using Shipping.Api.Contracts.Responses;
using Shipping.Api.Mappers;
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
        CreateShipmentRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = ShippingMapper.ToCreateShipmentCommand(request);
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
        return TypedResults.Ok(ShippingMapper.ToCreateShipmentResponse(result.OrderId, result.TrackingCode));
    }

    private static async Task<Ok<IReadOnlyList<ShipmentResponse>>> ListShipments(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var items = await queryDispatcher.ExecuteAsync(
            new ListShipmentsQuery(limit ?? 50, offset ?? 0),
            cancellationToken);
        IReadOnlyList<ShipmentResponse> response = items.Select(ShippingMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ShipmentResponse>, NotFound>> GetShipmentByOrder(
        Guid orderId,
        IQueryDispatcher queryDispatcher,
        CancellationToken cancellationToken)
    {
        var shipment = await queryDispatcher.ExecuteAsync(new GetShipmentByOrderIdQuery(orderId), cancellationToken);
        return shipment is null ? TypedResults.NotFound() : TypedResults.Ok(ShippingMapper.ToResponse(shipment));
    }

    private static async Task<Results<Ok<ShipmentResponse>, NotFound>> UpdateShipmentStatus(
        Guid shipmentId,
        UpdateShipmentStatusRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = ShippingMapper.ToUpdateShipmentStatusCommand(shipmentId, request);
        var updated = await commandDispatcher.ExecuteAsync(command, cancellationToken);
        return updated is null ? TypedResults.NotFound() : TypedResults.Ok(ShippingMapper.ToResponse(updated));
    }
}
