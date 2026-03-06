using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Commands;

public sealed record AddCartItemToCartCommand(Guid CartId, AddCartItemCommand Item) : ICommand<Unit>;
