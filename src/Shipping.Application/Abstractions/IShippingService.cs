using Shared.BuildingBlocks.Contracts;

namespace Shipping.Application;

public interface IShippingService
{
    Task<ShipmentResult> CreateShipmentAsync(ShippingCreateRequestedV1 request, CancellationToken cancellationToken);
}
