using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Order.Application;
using Order.Api.Contracts;
using Order.Application.Queries;
using Order.Application.Views;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(OrderRoutes.Base)
            .WithTags("Order")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder");

        group.MapGet("/", ListOrders)
            .WithName("ListOrders");

        group.MapGet("/{orderId:guid}", GetOrder)
            .WithName("GetOrder");

        return group;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderCommand command,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Accepted($"{OrderRoutes.Base}/{result.OrderId}", new { orderId = result.OrderId, status = result.Status });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(
                title: "Invalid cart",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    private static async Task<Results<Ok<object>, NotFound>> GetOrder(
        Guid orderId,
        IQueryDispatcher queryDispatcher,
        CancellationToken cancellationToken,
        [FromQuery(Name = "includeNonCompleted")] bool includeNonCompleted = false)
    {
        var order = await queryDispatcher.ExecuteAsync(new GetOrderByIdQuery(orderId), cancellationToken);
        if (order is null)
        {
            return TypedResults.NotFound();
        }

        if (!includeNonCompleted && !string.Equals(order.Status, "Completed", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok((object)order);
    }

    private static async Task<Ok<IReadOnlyList<OrderView>>> ListOrders(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        CancellationToken cancellationToken,
        [FromQuery(Name = "includeNonCompleted")] bool includeNonCompleted = false)
    {
        var safeLimit = Math.Clamp(limit ?? 50, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var orders = await queryDispatcher.ExecuteAsync(new GetOrdersQuery(safeLimit, safeOffset), cancellationToken);
        var filteredOrders = includeNonCompleted
            ? orders
            : orders.Where(order => string.Equals(order.Status, "Completed", StringComparison.OrdinalIgnoreCase)).ToList();

        return TypedResults.Ok(filteredOrders);
    }
}
