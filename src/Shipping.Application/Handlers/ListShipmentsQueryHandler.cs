using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Abstractions;
using Shipping.Application.Models;
using Shipping.Application.Queries;

namespace Shipping.Application.Handlers;

public sealed class ListShipmentsQueryHandler(IShippingService shippingService)
    : IQueryHandler<ListShipmentsQuery, IReadOnlyList<ShipmentView>>
{
    public Task<IReadOnlyList<ShipmentView>> HandleAsync(ListShipmentsQuery query, CancellationToken cancellationToken)
    {
        return shippingService.ListShipmentsAsync(query.Limit, query.Offset, query.SearchTerm, cancellationToken);
    }
}
