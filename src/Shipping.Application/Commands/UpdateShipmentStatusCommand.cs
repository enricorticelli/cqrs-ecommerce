using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Models;

namespace Shipping.Application.Commands;

public sealed record UpdateShipmentStatusCommand(Guid ShipmentId, string Status) : ICommand<ShipmentView?>;