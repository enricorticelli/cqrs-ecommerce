using Shared.BuildingBlocks.Contracts;

namespace Shipping.Application;

public sealed record ShipmentResult(Guid OrderId, string TrackingCode);

public interface IShippingService
{
    Task<ShipmentResult> CreateShipmentAsync(ShippingCreateRequestedV1 request, CancellationToken cancellationToken);
}
