using Shared.BuildingBlocks.Cqrs.Abstractions;
using Order.Application.Models;

namespace Order.Application.Commands;

public sealed record ManualCompleteOrderCommand(
    Guid OrderId,
    string? TrackingCode,
    string? TransactionId) : ICommand<ManualOrderActionResult>;
