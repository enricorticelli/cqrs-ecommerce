using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Models;

namespace Shipping.Application.Queries;

public sealed record GetShipmentByOrderIdQuery(Guid OrderId) : IQuery<ShipmentView?>;