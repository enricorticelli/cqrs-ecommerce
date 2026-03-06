using Cart.Application.Abstractions;
using Cart.Application.Commands;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Handlers;

public sealed class CheckoutCartCommandHandler(ICartService cartService)
    : ICommandHandler<CheckoutCartCommand, CartCheckedOutV1?>
{
    public Task<CartCheckedOutV1?> HandleAsync(CheckoutCartCommand command, CancellationToken cancellationToken)
    {
        return cartService.CheckoutAsync(command.CartId, cancellationToken);
    }
}
