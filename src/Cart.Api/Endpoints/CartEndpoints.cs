using Cart.Application;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Contracts;

namespace Cart.Api.Endpoints;

public static class CartEndpoints
{
    public static RouteGroupBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/carts")
            .WithTags("Cart");

        group.MapPost("/{cartId:guid}/items", AddItem)
            .WithName("AddCartItem");

        group.MapDelete("/{cartId:guid}/items/{productId:guid}", RemoveItem)
            .WithName("RemoveCartItem");

        group.MapGet("/{cartId:guid}", GetCart)
            .WithName("GetCart");

        group.MapPost("/{cartId:guid}/checkout", CheckoutCart)
            .WithName("CheckoutCart");

        return group;
    }

    private static async Task<IResult> AddItem(
        Guid cartId,
        AddCartItemCommand command,
        IValidator<AddCartItemCommand> validator,
        ICartService service,
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
                detail: "Invalid add item command",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?> { ["errors"] = errors });
        }

        await service.AddItemAsync(cartId, command, cancellationToken);
        return TypedResults.Ok((object)new { cartId, message = "Item added" });
    }

    private static async Task<Ok<object>> RemoveItem(Guid cartId, Guid productId, ICartService service, CancellationToken cancellationToken)
    {
        await service.RemoveItemAsync(cartId, productId, cancellationToken);
        return TypedResults.Ok((object)new { cartId, productId, message = "Item removed" });
    }

    private static async Task<Results<Ok<object>, NotFound>> GetCart(Guid cartId, ICartService service, CancellationToken cancellationToken)
    {
        var cart = await service.GetCartAsync(cartId, cancellationToken);
        if (cart is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok((object)cart);
    }

    private static async Task<Results<Ok<CartCheckedOutV1>, NotFound>> CheckoutCart(Guid cartId, ICartService service, CancellationToken cancellationToken)
    {
        var checkout = await service.CheckoutAsync(cartId, cancellationToken);
        return checkout is null ? TypedResults.NotFound() : TypedResults.Ok(checkout);
    }
}
