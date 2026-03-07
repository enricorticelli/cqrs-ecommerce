using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Order.Application;
using Order.Api.Contracts;
using Order.Application.Commands;
using Order.Application.Models;
using Order.Application.Queries;
using Order.Application.Views;
using Shared.BuildingBlocks.Api;
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
        group.MapPost("/{orderId:guid}/manual-complete", ManualCompleteOrder)
            .WithName("ManualCompleteOrder");
        group.MapPost("/{orderId:guid}/manual-cancel", ManualCancelOrder)
            .WithName("ManualCancelOrder");
        return group;
    }

    private static async Task<Results<Accepted<OrderCreatedResponse>, NotFound>> CreateOrder(
        CreateOrderCommand command,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        var response = new OrderCreatedResponse(result.OrderId, result.Status);
        return TypedResults.Accepted($"{OrderRoutes.Base}/{result.OrderId}", response);
    }

    private static async Task<Results<Ok<OrderView>, NotFound>> GetOrder(
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

        return TypedResults.Ok(order);
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

    private static async Task<Results<Ok<ManualCompleteOrderResponse>, NotFound, ProblemHttpResult>> ManualCompleteOrder(
        Guid orderId,
        ManualCompleteOrderRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.ExecuteAsync(
            new ManualCompleteOrderCommand(orderId, request.TrackingCode, request.TransactionId),
            cancellationToken);
        if (result.Outcome == ManualOrderActionOutcome.NotFound)
        {
            return TypedResults.NotFound();
        }

        if (result.Outcome == ManualOrderActionOutcome.Conflict)
        {
            return TypedResults.Problem(
                title: "Order in terminal state",
                detail: result.Detail ?? "Order cannot be manually completed because it is already terminal.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return TypedResults.Ok(new ManualCompleteOrderResponse(
            orderId,
            result.Status,
            result.TrackingCode ?? string.Empty,
            result.TransactionId ?? string.Empty,
            "manual"));
    }

    private static async Task<Results<Ok<ManualCancelOrderResponse>, NotFound, ProblemHttpResult>> ManualCancelOrder(
        Guid orderId,
        ManualCancelOrderRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.ExecuteAsync(
            new ManualCancelOrderCommand(orderId, request.Reason),
            cancellationToken);
        if (result.Outcome == ManualOrderActionOutcome.NotFound)
        {
            return TypedResults.NotFound();
        }

        if (result.Outcome == ManualOrderActionOutcome.Conflict)
        {
            return TypedResults.Problem(
                title: "Order in terminal state",
                detail: result.Detail ?? "Order cannot be manually cancelled because it is already failed.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return TypedResults.Ok(new ManualCancelOrderResponse(
            orderId,
            result.Status,
            result.Reason ?? string.Empty,
            "manual"));
    }
}
