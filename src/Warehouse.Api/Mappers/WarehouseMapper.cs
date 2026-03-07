using Shared.BuildingBlocks.Contracts.Integration;
using Warehouse.Api.Contracts.Requests;
using Warehouse.Api.Contracts.Responses;
using Warehouse.Application.Commands;
using Warehouse.Application.Models;

namespace Warehouse.Api.Mappers;

public static class WarehouseMapper
{
    public static ReserveStockCommand ToReserveStockCommand(ReserveStockRequest request)
    {
        var payload = new StockReserveRequestedV1(
            request.OrderId,
            request.Items.Select(item => new OrderItemDto(item.ProductId, item.Sku, item.Name, item.Quantity, item.UnitPrice)).ToArray());

        return new ReserveStockCommand(payload);
    }

    public static ReserveStockResponse ToReserveStockResponse(Guid orderId, bool reserved, string? reason)
        => new(orderId, reserved, reason);

    public static UpsertStockCommand ToUpsertStockCommand(UpsertStockRequest request)
        => new(new UpsertStockItem(request.ProductId, request.Sku, request.AvailableQuantity));

    public static UpsertStockResponse ToUpsertStockResponse(UpsertStockRequest request)
        => new(request.ProductId, request.Sku, request.AvailableQuantity);
}
