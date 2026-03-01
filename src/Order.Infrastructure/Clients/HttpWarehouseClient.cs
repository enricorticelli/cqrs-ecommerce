using System.Net.Http.Json;
using System.Text.Json;
using Order.Application;
using Shared.BuildingBlocks.Contracts;

namespace Order.Infrastructure.Clients;

public sealed class HttpWarehouseClient(IHttpClientFactory httpClientFactory) : IWarehouseClient
{
    public async Task<StockReservationDecision> ReserveStockAsync(Guid orderId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken)
    {
        var warehouseClient = httpClientFactory.CreateClient("warehouse");
        var reserveResponse = await warehouseClient.PostAsJsonAsync(
            "/v1/stock/reserve",
            new StockReserveRequestedV1(orderId, items),
            cancellationToken);

        reserveResponse.EnsureSuccessStatusCode();
        var payload = await reserveResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        var isReserved = payload.TryGetProperty("reserved", out var reservedElement) && reservedElement.GetBoolean();
        var reason = payload.TryGetProperty("reason", out var reasonElement)
            ? reasonElement.GetString()
            : null;

        return new StockReservationDecision(isReserved, reason);
    }
}
