using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using Warehouse.Api.Contracts;
using Warehouse.Api.Contracts.Requests;
using Warehouse.Api.Contracts.Responses;
using Warehouse.Api.Mappers;

namespace Warehouse.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static RouteGroupBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(WarehouseRoutes.Base)
            .WithTags("Warehouse")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapPost("/", UpsertStock)
            .WithName("UpsertStock");
        group.MapPost("/reserve", ReserveStock)
            .WithName("ReserveStock");
        return group;
    }

    private static async Task<Ok<ReserveStockResponse>> ReserveStock(ReserveStockRequest request, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var command = WarehouseMapper.ToReserveStockCommand(request);
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
        return TypedResults.Ok(WarehouseMapper.ToReserveStockResponse(result.OrderId, result.Reserved, result.Reason));
    }

    private static async Task<Ok<UpsertStockResponse>> UpsertStock(UpsertStockRequest request, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var command = WarehouseMapper.ToUpsertStockCommand(request);
        await commandDispatcher.ExecuteAsync(command, cancellationToken);
        return TypedResults.Ok(WarehouseMapper.ToUpsertStockResponse(request));
    }
}
