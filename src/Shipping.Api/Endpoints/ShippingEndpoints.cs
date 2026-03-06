using Microsoft.AspNetCore.Http.HttpResults;
using Shipping.Api.Contracts;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application;
using Shipping.Application.Commands;

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

        return group;
    }

    private static async Task<Ok<object>> CreateShipment(
        ShippingCreateRequestedV1 request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.ExecuteAsync(new CreateShipmentCommand(request), cancellationToken);
        return TypedResults.Ok((object)new { result.OrderId, result.TrackingCode });
    }
}
