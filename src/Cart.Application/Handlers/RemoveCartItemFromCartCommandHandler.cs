using Cart.Application.Abstractions;
using Cart.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Handlers;

public sealed class RemoveCartItemFromCartCommandHandler(ICartService cartService)
    : ICommandHandler<RemoveCartItemFromCartCommand, Unit>
{
    public async Task<Unit> HandleAsync(RemoveCartItemFromCartCommand command, CancellationToken cancellationToken)
    {
        await cartService.RemoveItemAsync(command.CartId, command.ProductId, cancellationToken);
        return Unit.Value;
    }
}
