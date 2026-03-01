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

        var internalGroup = app.MapGroup("/internal/seed")
            .WithTags("WarehouseInternal");

        internalGroup.MapPost("/stock", SeedStock)
            .WithName("SeedStock");

        return group;
    }

    private static async Task<Ok<object>> ReserveStock(StockReserveRequestedV1 request, IWarehouseService service, CancellationToken cancellationToken)
    {
        var result = await service.ReserveStockAsync(request, cancellationToken);
        return TypedResults.Ok((object)new { result.OrderId, result.Reserved, result.Reason });
    }

    private static async Task<Ok<object>> SeedStock(IWarehouseService service, CancellationToken cancellationToken)
    {
        var count = await service.SeedStockAsync(cancellationToken);
        return TypedResults.Ok((object)new { Seeded = count });
    }
}
