using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Contracts;
using Order.Api.Contracts.Requests;
using Order.Api.Contracts.Responses;
using Order.Api.Mappers;
using Order.Application.Models;
using Order.Application.Queries;
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
        CreateOrderRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = OrderMapper.ToCreateOrderCommand(request);
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        var response = OrderMapper.ToOrderCreatedResponse(result.OrderId, result.Status);
        return TypedResults.Accepted($"{OrderRoutes.Base}/{result.OrderId}", response);
    }

    private static async Task<Results<Ok<OrderResponse>, NotFound>> GetOrder(
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

        return TypedResults.Ok(OrderMapper.ToResponse(order));
    }

    private static async Task<Ok<IReadOnlyList<OrderResponse>>> ListOrders(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        string? searchTerm,
        CancellationToken cancellationToken,
        [FromQuery(Name = "includeNonCompleted")] bool includeNonCompleted = false)
    {
        var safeLimit = Math.Clamp(limit ?? 50, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var orders = await queryDispatcher.ExecuteAsync(new GetOrdersQuery(safeLimit, safeOffset, searchTerm), cancellationToken);
        var filteredOrders = includeNonCompleted
            ? orders
            : orders.Where(order => string.Equals(order.Status, "Completed", StringComparison.OrdinalIgnoreCase)).ToList();

        IReadOnlyList<OrderResponse> response = filteredOrders.Select(OrderMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ManualCompleteOrderResponse>, NotFound, ProblemHttpResult>> ManualCompleteOrder(
        Guid orderId,
        ManualCompleteOrderRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = OrderMapper.ToManualCompleteOrderCommand(orderId, request);
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
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

        return TypedResults.Ok(OrderMapper.ToManualCompleteOrderResponse(orderId, result));
    }

    private static async Task<Results<Ok<ManualCancelOrderResponse>, NotFound, ProblemHttpResult>> ManualCancelOrder(
        Guid orderId,
        ManualCancelOrderRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = OrderMapper.ToManualCancelOrderCommand(orderId, request);
        var result = await commandDispatcher.ExecuteAsync(command, cancellationToken);
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

        return TypedResults.Ok(OrderMapper.ToManualCancelOrderResponse(orderId, result));
    }
}
