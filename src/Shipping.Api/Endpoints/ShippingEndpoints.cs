using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Contracts;
using Shipping.Application;

namespace Shipping.Api.Endpoints;

public static class ShippingEndpoints
{
    public static RouteGroupBuilder MapShippingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/shipments")
            .WithTags("Shipping");

        group.MapPost("/", CreateShipment)
            .WithName("CreateShipment");

        return group;
    }

    private static async Task<Ok<object>> CreateShipment(
        ShippingCreateRequestedV1 request,
        IShippingService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateShipmentAsync(request, cancellationToken);
        return TypedResults.Ok((object)new { result.OrderId, result.TrackingCode });
    }
}
