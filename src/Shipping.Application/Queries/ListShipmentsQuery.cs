using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Models;

namespace Shipping.Application.Queries;

public sealed record ListShipmentsQuery(int Limit, int Offset, string? SearchTerm) : IQuery<IReadOnlyList<ShipmentView>>;
