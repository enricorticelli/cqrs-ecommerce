using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Contracts;
using Warehouse.Application;

namespace Warehouse.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static RouteGroupBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/stock")
            .WithTags("Warehouse");

        group.MapPost("/reserve", ReserveStock)
            .WithName("ReserveStock");

        return group;
    }

    private static async Task<Ok<object>> ReserveStock(StockReserveRequestedV1 request, IWarehouseService service, CancellationToken cancellationToken)
    {
        var result = await service.ReserveStockAsync(request, cancellationToken);
        return TypedResults.Ok((object)new { result.OrderId, result.Reserved, result.Reason });
    }

}
