using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Abstractions;
using Shipping.Application.Commands;
using Shipping.Application.Models;

namespace Shipping.Application.Handlers;

public sealed class CreateShipmentCommandHandler(IShippingService shippingService)
    : ICommandHandler<CreateShipmentCommand, ShipmentResult>
{
    public Task<ShipmentResult> HandleAsync(CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        return shippingService.CreateShipmentAsync(command.Request, cancellationToken);
    }
}
