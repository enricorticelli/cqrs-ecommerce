using Microsoft.AspNetCore.Http.HttpResults;
using Order.Application;
using Shared.BuildingBlocks.Http;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/orders")
            .WithTags("Order");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder");

        group.MapGet("/{orderId:guid}", GetOrder)
            .WithName("GetOrder");

        return group;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderCommand command,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return ProblemDetailsExtensions.ValidationProblem(errors, "Invalid create order command");
        }

        try
        {
            var result = await orderService.CreateOrderAsync(command, cancellationToken);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Accepted($"/v1/orders/{result.OrderId}", new { orderId = result.OrderId, status = result.Status });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Problem(
                title: "Invalid cart",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    private static async Task<Results<Ok<object>, NotFound>> GetOrder(Guid orderId, IOrderService orderService, CancellationToken cancellationToken)
    {
        var order = await orderService.GetOrderAsync(orderId, cancellationToken);
        return order is null ? TypedResults.NotFound() : TypedResults.Ok((object)order);
    }
}
