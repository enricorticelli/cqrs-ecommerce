using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Abstractions;
using Shipping.Application.Commands;
using Shipping.Application.Models;

namespace Shipping.Application.Handlers;

public sealed class UpdateShipmentStatusCommandHandler(IShippingService shippingService)
    : ICommandHandler<UpdateShipmentStatusCommand, ShipmentView?>
{
    public Task<ShipmentView?> HandleAsync(UpdateShipmentStatusCommand command, CancellationToken cancellationToken)
    {
        return shippingService.UpdateStatusAsync(command.ShipmentId, command.Status, cancellationToken);
    }
}