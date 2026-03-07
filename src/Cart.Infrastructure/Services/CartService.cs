using Cart.Application;
using Cart.Application.Abstractions;
using Cart.Application.Views;
using Cart.Domain.Aggregates;
using Cart.Domain.Events;
using Cart.Infrastructure.Persistence.ReadModels;
using Marten;
using Shared.BuildingBlocks.Contracts.Integration;
using Wolverine;

namespace Cart.Infrastructure.Services;

public sealed class CartService(
    IDocumentSession documentSession,
    ICartReadModelStore cartReadModelStore,
    IMessageBus bus) : ICartService
{
    public async Task AddItemAsync(Guid cartId, AddCartItemCommand command, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CartAggregate>(cartId, cancellationToken);
        CartCreated? createdEvent = null;
        if (!stream.Events.Any())
        {
            createdEvent = new CartCreated(cartId, command.UserId);
            stream.AppendOne(createdEvent);
        }
        else if (stream.Aggregate?.IsClosed == true)
        {
            throw new InvalidOperationException("Cart is closed and cannot be modified.");
        }

        var addedEvent = new CartItemAdded(cartId, command.ProductId, command.Sku, command.Name, command.Quantity, command.UnitPrice);
        stream.AppendOne(addedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        if (createdEvent is not null)
        {
            await bus.PublishAsync(createdEvent);
        }

        await bus.PublishAsync(addedEvent);
    }

    public async Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CartAggregate>(cartId, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsClosed == true)
        {
            return;
        }

        var removedEvent = new CartItemRemoved(cartId, productId);
        stream.AppendOne(removedEvent);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(removedEvent);
    }

    public async Task<CartView?> GetCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var readModel = await cartReadModelStore.GetAsync(cartId, cancellationToken);
        if (readModel is null)
        {
            return null;
        }

        return new CartView(readModel.CartId, readModel.UserId, readModel.Items, readModel.TotalAmount);
    }

    public async Task<CartCheckedOutV1?> CheckoutAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var readModel = await cartReadModelStore.GetAsync(cartId, cancellationToken);
        if (readModel is null || readModel.Items.Count == 0)
        {
            return null;
        }

        var stream = await documentSession.Events.FetchForWriting<CartAggregate>(cartId, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsClosed == true)
        {
            return null;
        }

        return new CartCheckedOutV1(
            cartId,
            Guid.NewGuid(),
            readModel.UserId,
            readModel.Items,
            readModel.TotalAmount);
    }

    public async Task RotateCartAfterOrderCompletionAsync(Guid cartId, Guid userId, Guid orderId, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CartAggregate>(cartId, cancellationToken);
        if (stream.Events.Any() && stream.Aggregate?.IsClosed != true)
        {
            var checkedOutEvent = new CartCheckedOut(cartId, orderId);
            stream.AppendOne(checkedOutEvent);
            await documentSession.SaveChangesAsync(cancellationToken);
            await bus.PublishAsync(checkedOutEvent);
        }

        var newCartId = Guid.NewGuid();
        var nextCartStream = await documentSession.Events.FetchForWriting<CartAggregate>(newCartId, cancellationToken);
        if (!nextCartStream.Events.Any())
        {
            var createdEvent = new CartCreated(newCartId, userId);
            nextCartStream.AppendOne(createdEvent);
            await documentSession.SaveChangesAsync(cancellationToken);
            await bus.PublishAsync(createdEvent);
        }
    }

    internal Task UpsertReadModelAsync(CartReadModelRow row, CancellationToken cancellationToken)
        => cartReadModelStore.UpsertAsync(row, cancellationToken);

    internal Task<CartReadModelRow?> GetReadModelAsync(Guid cartId, CancellationToken cancellationToken)
        => cartReadModelStore.GetAsync(cartId, cancellationToken);
}
