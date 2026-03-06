using Shared.BuildingBlocks.Cqrs;

namespace Order.Application;

public sealed record GetOrdersQuery(int Limit, int Offset) : IQuery<IReadOnlyList<OrderView>>;
