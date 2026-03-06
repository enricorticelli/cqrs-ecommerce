using Shared.BuildingBlocks.Cqrs.Abstractions;
using Warehouse.Application.Models;

namespace Warehouse.Application.Commands;

public sealed record UpsertStockCommand(UpsertStockItem Item) : ICommand<Unit>;
