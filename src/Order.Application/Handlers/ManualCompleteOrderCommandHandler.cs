using Order.Application.Abstractions;
using Order.Application.Commands;
using Order.Application.Models;
using Order.Domain.Enums;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application.Handlers;

public sealed class ManualCompleteOrderCommandHandler(
    IOrderStateReader orderStateReader,
    IOrderStateStore orderStateStore,
    IOrderEventPublisher orderEventPublisher)
    : ICommandHandler<ManualCompleteOrderCommand, ManualOrderActionResult>
{
    public async Task<ManualOrderActionResult> HandleAsync(
        ManualCompleteOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderState = await orderStateReader.GetAsync(command.OrderId, cancellationToken);
        if (orderState is null)
        {
            return ManualOrderActionResult.NotFound();
        }

        if (orderState.Status is OrderStatus.Completed or OrderStatus.Failed)
        {
            return ManualOrderActionResult.Conflict(
                "Order cannot be manually completed because it is already terminal.");
        }

        var trackingCode = string.IsNullOrWhiteSpace(command.TrackingCode)
            ? $"MAN-TRK-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}"
            : command.TrackingCode.Trim();

        var transactionId = string.IsNullOrWhiteSpace(command.TransactionId)
            ? (string.IsNullOrWhiteSpace(orderState.TransactionId)
                ? $"MAN-TX-{Guid.NewGuid():N}"
                : orderState.TransactionId)
            : command.TransactionId.Trim();

        await orderStateStore.MarkCompletedAsync(command.OrderId, trackingCode, transactionId, cancellationToken);
        await orderEventPublisher.PublishOrderCompletedAsync(
            command.OrderId,
            orderState.CartId,
            orderState.UserId,
            trackingCode,
            transactionId);

        return new ManualOrderActionResult(
            ManualOrderActionOutcome.Success,
            "Completed",
            trackingCode,
            transactionId,
            null,
            null);
    }
}
