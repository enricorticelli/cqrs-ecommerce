using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Commands;

public sealed record CheckoutCartCommand(Guid CartId) : ICommand<CartCheckedOutV1?>;
