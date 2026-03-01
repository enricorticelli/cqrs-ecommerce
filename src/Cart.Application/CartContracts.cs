using Shared.BuildingBlocks.Contracts;

namespace Cart.Application;

public sealed record CartView(Guid CartId, Guid UserId, IReadOnlyList<OrderItemDto> Items, decimal TotalAmount);

public interface ICartService
{
    Task AddItemAsync(Guid cartId, AddCartItemCommand command, CancellationToken cancellationToken);
    Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken cancellationToken);
    Task<CartView?> GetCartAsync(Guid cartId, CancellationToken cancellationToken);
    Task<CartCheckedOutV1?> CheckoutAsync(Guid cartId, CancellationToken cancellationToken);
}
