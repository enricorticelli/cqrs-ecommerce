using Cart.Domain.Events;
using Cart.Infrastructure.Persistence.ReadModels;
using Cart.Infrastructure.Services;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Cart.Infrastructure.Messaging.Handlers;

public sealed class CartDomainEventProjectionHandler
{
    public static Task Handle(CartCreated message, CartService cartService, CancellationToken cancellationToken)
    {
        var row = new CartReadModelRow(message.CartId, message.UserId, Array.Empty<OrderItemDto>(), 0m);
        return cartService.UpsertReadModelAsync(row, cancellationToken);
    }

    public static async Task Handle(CartItemAdded message, CartService cartService, CancellationToken cancellationToken)
    {
        var current = await cartService.GetReadModelAsync(message.CartId, cancellationToken);
        if (current is null)
        {
            return;
        }

        var items = current.Items.ToList();
        var existingIndex = items.FindIndex(x => x.ProductId == message.ProductId);
        if (existingIndex >= 0)
        {
            var existing = items[existingIndex];
            items[existingIndex] = existing with { Quantity = existing.Quantity + message.Quantity };
        }
        else
        {
            items.Add(new OrderItemDto(
                message.ProductId,
                message.Sku,
                message.Name,
                message.Quantity,
                message.UnitPrice));
        }

        var total = items.Sum(x => x.Quantity * x.UnitPrice);
        await cartService.UpsertReadModelAsync(
            current with { Items = items, TotalAmount = total },
            cancellationToken);
    }

    public static async Task Handle(CartItemRemoved message, CartService cartService, CancellationToken cancellationToken)
    {
        var current = await cartService.GetReadModelAsync(message.CartId, cancellationToken);
        if (current is null)
        {
            return;
        }

        var items = current.Items.Where(x => x.ProductId != message.ProductId).ToArray();
        var total = items.Sum(x => x.Quantity * x.UnitPrice);
        await cartService.UpsertReadModelAsync(
            current with { Items = items, TotalAmount = total },
            cancellationToken);
    }

    public static Task Handle(CartCheckedOut message, CartService cartService, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
