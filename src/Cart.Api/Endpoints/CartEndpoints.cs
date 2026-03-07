using Cart.Api.Contracts;
using Cart.Api.Contracts.Requests;
using Cart.Api.Contracts.Responses;
using Cart.Api.Mappers;
using Cart.Application.Commands;
using Cart.Application.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Api.Endpoints;

public static class CartEndpoints
{
    public static RouteGroupBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CartRoutes.Base)
            .WithTags("Cart")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

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

    private static async Task<Ok<AddCartItemResponse>> AddItem(
        Guid cartId,
        AddCartItemRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = CartMapper.ToAddCartItemToCartCommand(cartId, request);
        await commandDispatcher.ExecuteAsync(command, cancellationToken);
        return TypedResults.Ok(CartMapper.ToAddCartItemResponse(cartId));
    }

    private static async Task<Ok<RemoveCartItemResponse>> RemoveItem(Guid cartId, Guid productId, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        await commandDispatcher.ExecuteAsync(new RemoveCartItemFromCartCommand(cartId, productId), cancellationToken);
        return TypedResults.Ok(CartMapper.ToRemoveCartItemResponse(cartId, productId));
    }

    private static async Task<Results<Ok<CartResponse>, NotFound>> GetCart(Guid cartId, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var cart = await queryDispatcher.ExecuteAsync(new GetCartByIdQuery(cartId), cancellationToken);
        if (cart is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(CartMapper.ToResponse(cart));
    }

    private static async Task<Results<Ok<CheckoutCartResponse>, NotFound>> CheckoutCart(Guid cartId, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var checkout = await commandDispatcher.ExecuteAsync(new CheckoutCartCommand(cartId), cancellationToken);
        return checkout is null ? TypedResults.NotFound() : TypedResults.Ok(CartMapper.ToResponse(checkout));
    }
}
