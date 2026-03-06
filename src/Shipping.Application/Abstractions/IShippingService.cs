using Shared.BuildingBlocks.Contracts.Integration;
using Shipping.Application.Models;

namespace Shipping.Application.Abstractions;

public interface IShippingService
{
    Task<ShipmentResult> CreateShipmentAsync(ShippingCreateRequestedV1 request, CancellationToken cancellationToken);
}
