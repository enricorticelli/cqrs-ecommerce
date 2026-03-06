using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using Warehouse.Application.Models;

namespace Warehouse.Application.Commands;

public sealed record ReserveStockCommand(StockReserveRequestedV1 Request) : ICommand<StockReservationResult>;
