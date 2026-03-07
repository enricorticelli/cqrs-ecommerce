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

    public async Task<IReadOnlyList<ShipmentView>> ListShipmentsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);
        var normalizedSearch = searchTerm?.Trim();
        IQueryable<ShipmentAggregate> query = querySession.Query<ShipmentAggregate>();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            var loweredSearch = normalizedSearch.ToLowerInvariant();
            if (Guid.TryParse(normalizedSearch, out var parsedGuid))
            {
                query = query.Where(x =>
                    x.Id == parsedGuid ||
                    x.OrderId == parsedGuid ||
                    x.UserId == parsedGuid ||
                    x.TrackingCode.ToLower().Contains(loweredSearch) ||
                    x.Status.ToLower().Contains(loweredSearch));
            }
            else
            {
                query = query.Where(x =>
                    x.TrackingCode.ToLower().Contains(loweredSearch) ||
                    x.Status.ToLower().Contains(loweredSearch));
            }
        }

        var rows = await query
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
