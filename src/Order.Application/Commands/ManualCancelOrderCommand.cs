using Shared.BuildingBlocks.Cqrs.Abstractions;
using Order.Application.Models;

namespace Order.Application.Commands;

public sealed record ManualCancelOrderCommand(
    Guid OrderId,
    string? Reason) : ICommand<ManualOrderActionResult>;
