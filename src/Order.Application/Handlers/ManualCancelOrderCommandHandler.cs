using Order.Application.Abstractions;
using Order.Application.Commands;
using Order.Application.Models;
using Order.Domain.Enums;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Handlers;

public sealed class ManualCancelOrderCommandHandler(
    IOrderStateReader orderStateReader,
    IOrderStateStore orderStateStore,
    IOrderEventPublisher orderEventPublisher)
    : ICommandHandler<ManualCancelOrderCommand, ManualOrderActionResult>
{
    public async Task<ManualOrderActionResult> HandleAsync(
        ManualCancelOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderState = await orderStateReader.GetAsync(command.OrderId, cancellationToken);
        if (orderState is null)
        {
            return ManualOrderActionResult.NotFound();
        }

        if (orderState.Status is OrderStatus.Failed)
        {
            return ManualOrderActionResult.Conflict(
                "Order cannot be manually cancelled because it is already failed.");
        }

        var reason = string.IsNullOrWhiteSpace(command.Reason)
            ? "Cancelled by backoffice"
            : command.Reason.Trim();

        await orderStateStore.MarkFailedAsync(command.OrderId, reason, cancellationToken);
        await orderEventPublisher.PublishOrderFailedAsync(command.OrderId, reason);

        return new ManualOrderActionResult(
            ManualOrderActionOutcome.Success,
            "Failed",
            null,
            null,
            reason,
            null);
    }
}
