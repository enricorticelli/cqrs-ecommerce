using Cart.Api.Contracts;
using Cart.Api.Contracts.Requests;
using Cart.Api.Contracts.Responses;
using Shared.BuildingBlocks.Api;

namespace Cart.Api.Endpoints;

public static class CartEndpoints
{
    public static RouteGroupBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CartRoutes.Base)
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

    private static IResult AddItem(Guid cartId, AddCartItemRequest request)
    {
        _ = request;
        return Results.Ok(new AddCartItemResponse(cartId, "Item added (stub)."));
    }

    private static IResult RemoveItem(Guid cartId, Guid productId)
    {
        return Results.Ok(new RemoveCartItemResponse(cartId, productId, "Item removed (stub)."));
    }

    private static IResult GetCart(Guid cartId)
    {
        var items = new[]
        {
            new CartItemResponse(Guid.NewGuid(), "STUB-SKU", "Stub item", 1, 10m)
        };
        return Results.Ok(new CartResponse(cartId, Guid.NewGuid(), items, items.Sum(i => i.Quantity * i.UnitPrice)));
    }

    private static IResult CheckoutCart(Guid cartId)
    {
        var items = new[]
        {
            new CartItemResponse(Guid.NewGuid(), "STUB-SKU", "Stub item", 1, 10m)
        };
        return Results.Ok(new CheckoutCartResponse(cartId, Guid.NewGuid(), Guid.NewGuid(), items, items.Sum(i => i.Quantity * i.UnitPrice)));
    }
}
