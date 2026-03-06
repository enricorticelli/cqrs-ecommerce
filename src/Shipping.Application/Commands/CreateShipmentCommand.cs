using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using Shipping.Application.Models;

namespace Shipping.Application.Commands;

public sealed record CreateShipmentCommand(ShippingCreateRequestedV1 Request) : ICommand<ShipmentResult>;
