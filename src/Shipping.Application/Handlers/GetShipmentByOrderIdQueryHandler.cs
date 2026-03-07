using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Abstractions;
using Shipping.Application.Models;
using Shipping.Application.Queries;

namespace Shipping.Application.Handlers;

public sealed class GetShipmentByOrderIdQueryHandler(IShippingService shippingService)
    : IQueryHandler<GetShipmentByOrderIdQuery, ShipmentView?>
{
    public Task<ShipmentView?> HandleAsync(GetShipmentByOrderIdQuery query, CancellationToken cancellationToken)
    {
        return shippingService.GetByOrderIdAsync(query.OrderId, cancellationToken);
    }
}