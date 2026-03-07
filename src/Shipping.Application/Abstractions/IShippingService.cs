using Shared.BuildingBlocks.Contracts.Integration;
using Shipping.Application.Models;

namespace Shipping.Application.Abstractions;

public interface IShippingService
{
    Task<ShipmentResult> CreateShipmentAsync(ShippingCreateRequestedV1 request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ShipmentView>> ListShipmentsAsync(int limit, int offset, CancellationToken cancellationToken);
    Task<ShipmentView?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<ShipmentView?> UpdateStatusAsync(Guid shipmentId, string status, CancellationToken cancellationToken);
}
