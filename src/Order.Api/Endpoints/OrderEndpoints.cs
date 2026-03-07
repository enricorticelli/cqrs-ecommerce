using Order.Api.Contracts;
using Order.Api.Contracts.Requests;
using Order.Api.Contracts.Responses;
using Shared.BuildingBlocks.Api;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(OrderRoutes.Base)
            .WithTags("Order");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder");
        group.MapGet("/", ListOrders)
            .WithName("ListOrders");
        group.MapGet("/{orderId:guid}", GetOrder)
            .WithName("GetOrder");
        group.MapPost("/{orderId:guid}/manual-complete", ManualCompleteOrder)
            .WithName("ManualCompleteOrder");
        group.MapPost("/{orderId:guid}/manual-cancel", ManualCancelOrder)
            .WithName("ManualCancelOrder");
        return group;
    }

    private static IResult CreateOrder(CreateOrderRequest request)
    {
        var orderId = Guid.NewGuid();
        _ = request;
        return Results.Created($"{OrderRoutes.Base}/{orderId}", new OrderCreatedResponse(orderId, "Pending"));
    }

    private static IResult ListOrders()
    {
        return Results.Ok(new[] { BuildOrderResponse(Guid.NewGuid()) });
    }

    private static IResult GetOrder(Guid orderId)
    {
        return Results.Ok(BuildOrderResponse(orderId));
    }

    private static IResult ManualCompleteOrder(Guid orderId, ManualCompleteOrderRequest request)
    {
        return Results.Ok(new ManualCompleteOrderResponse(
            orderId,
            "Completed",
            request.TrackingCode ?? "TRK-STUB",
            request.TransactionId ?? "TX-STUB",
            "manual"));
    }

    private static IResult ManualCancelOrder(Guid orderId, ManualCancelOrderRequest request)
    {
        return Results.Ok(new ManualCancelOrderResponse(orderId, "Cancelled", request.Reason ?? "Cancelled manually", "manual"));
    }

    private static OrderResponse BuildOrderResponse(Guid orderId)
    {
        var items = new[]
        {
            new OrderItemResponse(Guid.NewGuid(), "STUB-SKU", "Stub item", 1, 10m)
        };
        var customer = new OrderCustomerResponse("Stub", "User", "stub@example.com", "+390000000000");
        var address = new OrderAddressResponse("Stub street 1", "Rome", "00100", "IT");
        return new OrderResponse(
            orderId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "authenticated",
            "card",
            Guid.NewGuid(),
            null,
            customer,
            address,
            address,
            "Pending",
            items.Sum(i => i.Quantity * i.UnitPrice),
            items,
            string.Empty,
            string.Empty,
            string.Empty);
    }
}
