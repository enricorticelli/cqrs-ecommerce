using Cart.Api.Contracts.Requests;
using Cart.Api.Contracts.Responses;
using Cart.Application;
using Cart.Application.Commands;
using Cart.Application.Views;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Cart.Api.Mappers;

public static class CartMapper
{
    public static AddCartItemToCartCommand ToAddCartItemToCartCommand(Guid cartId, AddCartItemRequest request)
    {
        var command = new AddCartItemCommand(
            request.UserId,
            request.ProductId,
            request.Sku,
            request.Name,
            request.Quantity,
            request.UnitPrice);

        return new AddCartItemToCartCommand(cartId, command);
    }

    public static AddCartItemResponse ToAddCartItemResponse(Guid cartId)
        => new(cartId, "Item added");

    public static RemoveCartItemResponse ToRemoveCartItemResponse(Guid cartId, Guid productId)
        => new(cartId, productId, "Item removed");

    public static CartResponse ToResponse(CartView view)
        => new(
            view.CartId,
            view.UserId,
            view.Items.Select(item => new CartItemResponse(item.ProductId, item.Sku, item.Name, item.Quantity, item.UnitPrice)).ToArray(),
            view.TotalAmount);

    public static CheckoutCartResponse ToResponse(CartCheckedOutV1 view)
        => new(
            view.CartId,
            view.OrderId,
            view.UserId,
            view.Items.Select(item => new CartItemResponse(item.ProductId, item.Sku, item.Name, item.Quantity, item.UnitPrice)).ToArray(),
            view.TotalAmount);
}
