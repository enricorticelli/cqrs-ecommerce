using Marten;
using Shared.BuildingBlocks.Contracts;
using Warehouse.Application;
using Warehouse.Domain;

namespace Warehouse.Infrastructure;

public sealed class WarehouseService(IDocumentSession documentSession) : IWarehouseService
{
    public async Task<StockReservationResult> ReserveStockAsync(StockReserveRequestedV1 request, CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(i => i.ProductId).ToArray();
        var docs = await documentSession.Query<StockDocument>()
            .Where(x => productIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
        var byId = docs.ToDictionary(x => x.Id, x => x);

        var missing = request.Items.Where(i => !byId.ContainsKey(i.ProductId) || byId[i.ProductId].AvailableQuantity < i.Quantity).ToList();
        if (missing.Count > 0)
        {
            return new StockReservationResult(request.OrderId, false, "Stock not available");
        }

        foreach (var item in request.Items)
        {
            byId[item.ProductId].AvailableQuantity -= item.Quantity;
        }

        await documentSession.SaveChangesAsync(cancellationToken);
        return new StockReservationResult(request.OrderId, true);
    }

    public async Task<int> SeedStockAsync(CancellationToken cancellationToken)
    {
        var seed = new[]
        {
            new StockDocument { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Sku = "SKU-KEYBOARD-001", AvailableQuantity = 100 },
            new StockDocument { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Sku = "SKU-MOUSE-001", AvailableQuantity = 100 },
            new StockDocument { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Sku = "SKU-HEADSET-001", AvailableQuantity = 100 }
        };

        foreach (var stock in seed)
        {
            documentSession.Store(stock);
        }

        await documentSession.SaveChangesAsync(cancellationToken);
        return seed.Length;
    }
}
