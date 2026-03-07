using Shared.BuildingBlocks.Api;
using Warehouse.Api.Contracts;
using Warehouse.Api.Contracts.Requests;
using Warehouse.Api.Contracts.Responses;

namespace Warehouse.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static RouteGroupBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(WarehouseRoutes.Base)
            .WithTags("Warehouse");

        group.MapPost("/", UpsertStock)
            .WithName("UpsertStock");
        group.MapPost("/reserve", ReserveStock)
            .WithName("ReserveStock");
        return group;
    }

    private static IResult UpsertStock(UpsertStockRequest request)
    {
        return Results.Ok(new UpsertStockResponse(request.ProductId, request.Sku, request.AvailableQuantity));
    }

    private static IResult ReserveStock(ReserveStockRequest request)
    {
        var reserved = request.Items.All(item => item.Quantity > 0);
        return Results.Ok(new ReserveStockResponse(request.OrderId, reserved, reserved ? null : "Invalid quantity"));
    }
}
