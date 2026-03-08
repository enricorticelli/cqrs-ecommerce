using Order.Api.Contracts;
using Order.Api.Contracts.Requests;
using Order.Api.Contracts.Responses;
using Order.Api.Mappers;
using Order.Application.Abstractions.Commands;
using Order.Application.Abstractions.Queries;
using Order.Application.Commands;
using Shared.BuildingBlocks.Api.Correlation;
using Shared.BuildingBlocks.Api.Errors;

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

    private static async Task<IResult> CreateOrder(
        CreateOrderRequest request,
        IOrderCommandService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var order = await service.CreateAsync(request.ToCreateCommand(correlationId), cancellationToken);
            return Results.Created($"{OrderRoutes.Base}/{order.Id}", new OrderCreatedResponse(order.Id, order.Status));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> ListOrders(IOrderQueryService service, CancellationToken cancellationToken)
    {
        var orders = await service.ListAsync(cancellationToken);
        return Results.Ok(orders.Select(x => x.ToResponse()));
    }

    private static async Task<IResult> GetOrder(Guid orderId, IOrderQueryService service, CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.GetByIdAsync(orderId, cancellationToken);
            return Results.Ok(order.ToResponse());
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> ManualCompleteOrder(
        Guid orderId,
        ManualCompleteOrderRequest request,
        IOrderCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.ManualCompleteAsync(new ManualCompleteOrderCommand(orderId, request.TrackingCode, request.TransactionId), cancellationToken);
            return Results.Ok(new ManualCompleteOrderResponse(order.Id, order.Status, order.TrackingCode, order.TransactionId, "manual"));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> ManualCancelOrder(
        Guid orderId,
        ManualCancelOrderRequest request,
        IOrderCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.ManualCancelAsync(new ManualCancelOrderCommand(orderId, request.Reason), cancellationToken);
            return Results.Ok(new ManualCancelOrderResponse(order.Id, order.Status, order.FailureReason, "manual"));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }
}
