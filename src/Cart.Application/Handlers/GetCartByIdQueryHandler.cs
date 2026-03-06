using Cart.Application.Abstractions;
using Cart.Application.Queries;
using Cart.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Handlers;

public sealed class GetCartByIdQueryHandler(ICartService cartService)
    : IQueryHandler<GetCartByIdQuery, CartView?>
{
    public Task<CartView?> HandleAsync(GetCartByIdQuery query, CancellationToken cancellationToken)
    {
        return cartService.GetCartAsync(query.CartId, cancellationToken);
    }
}
