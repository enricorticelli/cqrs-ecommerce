using System.Net.Http.Json;
using System.Text.Json;
using Order.Application;
using Order.Application.Abstractions;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.Clients;

public sealed class HttpShippingClient(IHttpClientFactory httpClientFactory) : IShippingClient
{
    public async Task<string> CreateShipmentAsync(Guid orderId, Guid userId, IReadOnlyList<OrderItemDto> items, CancellationToken cancellationToken)
    {
        var shippingClient = httpClientFactory.CreateClient("shipping");
        var shippingResponse = await shippingClient.PostAsJsonAsync(
            "/v1/shipments",
            new ShippingCreateRequestedV1(orderId, userId, items),
            cancellationToken);

        shippingResponse.EnsureSuccessStatusCode();
        var payload = await shippingResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        return payload.TryGetProperty("trackingCode", out var trackingElement)
            ? trackingElement.GetString() ?? string.Empty
            : string.Empty;
    }
}
