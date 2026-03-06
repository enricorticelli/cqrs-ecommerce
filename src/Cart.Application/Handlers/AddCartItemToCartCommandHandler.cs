using Cart.Application.Abstractions;
using Cart.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Handlers;

public sealed class AddCartItemToCartCommandHandler(ICartService cartService)
    : ICommandHandler<AddCartItemToCartCommand, Unit>
{
    public async Task<Unit> HandleAsync(AddCartItemToCartCommand command, CancellationToken cancellationToken)
    {
        await cartService.AddItemAsync(command.CartId, command.Item, cancellationToken);
        return Unit.Value;
    }
}
