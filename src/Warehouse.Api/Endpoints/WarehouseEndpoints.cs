using Warehouse.Api.Contracts;
using Warehouse.Api.Contracts.Requests;
using Warehouse.Api.Contracts.Responses;
using Warehouse.Api.Mappers;
using Warehouse.Application.Abstractions.Commands;
using Shared.BuildingBlocks.Api.Errors;

namespace Warehouse.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static RouteGroupBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(WarehouseRoutes.AdminBase)
            .WithTags("Warehouse")
            .RequireAuthorization("AdminPolicy");

        group.MapPost("/", UpsertStock)
            .WithName("AdminUpsertStock");
        return group;
    }

    private static async Task<IResult> UpsertStock(
        UpsertStockRequest request,
        IWarehouseCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.UpsertStockAsync(request.ToCommand(), cancellationToken);
            return Results.Ok(result.ToResponse());
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

}
