using Order.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Queries;

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderView?>;
