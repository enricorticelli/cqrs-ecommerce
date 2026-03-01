using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Order.Application;

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
        IValidator<CreateOrderCommand> validator,
        IOrderService orderService,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return TypedResults.Problem(
                title: "Validation error",
                detail: "Invalid create order command",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?> { ["errors"] = errors });
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
