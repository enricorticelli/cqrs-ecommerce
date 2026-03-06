using Order.Application.Views;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Queries;

public sealed record GetOrdersQuery(int Limit, int Offset) : IQuery<IReadOnlyList<OrderView>>;
