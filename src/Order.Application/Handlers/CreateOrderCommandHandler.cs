using Order.Application.Abstractions;
using Order.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Handlers;

public sealed class CreateOrderCommandHandler(IOrderCommandService orderCommandService)
    : ICommandHandler<CreateOrderCommand, OrderCreationResult?>
{
    public Task<OrderCreationResult?> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        return orderCommandService.CreateOrderAsync(command, cancellationToken);
    }
}
