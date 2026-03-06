using Shared.BuildingBlocks.Contracts.Integration;
using Shipping.Application.Abstractions;

namespace Shipping.Infrastructure.Messaging.Handlers;

public sealed class ShippingCreateRequestedHandler
{
    public static async Task Handle(ShippingCreateRequestedV1 message, IShippingService shippingService, CancellationToken cancellationToken)
    {
        await shippingService.CreateShipmentAsync(message, cancellationToken);
    }
}
