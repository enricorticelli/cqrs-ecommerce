using Cart.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Cart.Application.Queries;

public sealed record GetCartByIdQuery(Guid CartId) : IQuery<CartView?>;
