using Cart.Api.Contracts;
using Cart.Api.Contracts.Requests;
using Cart.Api.Contracts.Responses;
using Cart.Api.Mappers;
using Cart.Application.Abstractions.Commands;
using Cart.Application.Abstractions.Queries;
using Cart.Application.Commands;
using Shared.BuildingBlocks.Api.Errors;

namespace Cart.Api.Endpoints;

public static class CartEndpoints
{
    public static RouteGroupBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CartRoutes.StoreBase)
            .WithTags("Cart");

        group.MapPost("/{cartId:guid}/items", AddItem)
            .WithName("StoreAddCartItem");
        group.MapDelete("/{cartId:guid}/items/{productId:guid}", RemoveItem)
            .WithName("StoreRemoveCartItem");
        group.MapGet("/{cartId:guid}", GetCart)
            .WithName("StoreGetCart");
        return group;
    }

    private static async Task<IResult> AddItem(
        Guid cartId,
        AddCartItemRequest request,
        ICartCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            await service.AddItemAsync(request.ToCommand(cartId), cancellationToken);
            return Results.Ok(new AddCartItemResponse(cartId, "Item added."));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> RemoveItem(
        Guid cartId,
        Guid productId,
        ICartCommandService service,
        CancellationToken cancellationToken)
    {
        try
        {
            await service.RemoveItemAsync(new RemoveCartItemCommand(cartId, productId), cancellationToken);
            return Results.Ok(new RemoveCartItemResponse(cartId, productId, "Item removed."));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> GetCart(
        Guid cartId,
        ICartQueryService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var cart = await service.GetByIdAsync(cartId, cancellationToken);
            return Results.Ok(cart.ToResponse());
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

}
