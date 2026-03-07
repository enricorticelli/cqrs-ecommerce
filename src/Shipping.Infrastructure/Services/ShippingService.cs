using Marten;
using Shared.BuildingBlocks.Contracts.Integration;
using Shipping.Application.Abstractions;
using Shipping.Application.Models;
using Shipping.Domain.Aggregates;
using Shipping.Domain.Enums;
using Wolverine;

namespace Shipping.Infrastructure.Services;

public sealed class ShippingService(IDocumentSession documentSession, IQuerySession querySession, IMessageBus bus) : IShippingService
{
    public async Task<ShipmentResult> CreateShipmentAsync(ShippingCreateRequestedV1 request, CancellationToken cancellationToken)
    {
        var trackingCode = $"TRK-{Guid.NewGuid():N}"[..16];
        var nowUtc = DateTimeOffset.UtcNow;

        documentSession.Store(new ShipmentAggregate
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            UserId = request.UserId,
            TrackingCode = trackingCode,
            Status = ShipmentStatuses.Created,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc,
            DeliveredAtUtc = null
        });

        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(new ShippingCreatedV1(request.OrderId, trackingCode));

        return new ShipmentResult(request.OrderId, trackingCode);
    }

    public async Task<IReadOnlyList<ShipmentView>> ListShipmentsAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);

        var rows = await querySession.Query<ShipmentAggregate>()
            .OrderByDescending(x => x.UpdatedAtUtc)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return rows.Select(MapToView).ToList();
    }

    public async Task<ShipmentView?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var row = await querySession.Query<ShipmentAggregate>()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.UpdatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : MapToView(row);
    }

    public async Task<ShipmentView?> UpdateStatusAsync(Guid shipmentId, string status, CancellationToken cancellationToken)
    {
        var normalizedStatus = (status ?? string.Empty).Trim();
        if (!ShipmentStatuses.IsSupported(normalizedStatus))
        {
            throw new InvalidOperationException($"Unsupported shipment status '{status}'.");
        }

        var shipment = await documentSession.LoadAsync<ShipmentAggregate>(shipmentId, cancellationToken);
        if (shipment is null)
        {
            return null;
        }

        shipment.Status = normalizedStatus;
        shipment.UpdatedAtUtc = DateTimeOffset.UtcNow;
        shipment.DeliveredAtUtc = normalizedStatus == ShipmentStatuses.Delivered
            ? shipment.UpdatedAtUtc
            : null;

        documentSession.Store(shipment);
        await documentSession.SaveChangesAsync(cancellationToken);

        return MapToView(shipment);
    }

    private static ShipmentView MapToView(ShipmentAggregate model)
    {
        return new ShipmentView(
            model.Id,
            model.OrderId,
            model.UserId,
            model.TrackingCode,
            model.Status,
            model.CreatedAtUtc,
            model.UpdatedAtUtc,
            model.DeliveredAtUtc);
    }
}
