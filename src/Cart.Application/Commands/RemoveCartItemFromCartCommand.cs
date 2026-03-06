using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Commands;

public sealed record RemoveCartItemFromCartCommand(Guid CartId, Guid ProductId) : ICommand<Unit>;
