using System.Net;
using System.Text.Json;
using Order.Application;

namespace Order.Infrastructure.Clients;

public sealed class HttpCartSnapshotClient(IHttpClientFactory httpClientFactory) : ICartSnapshotClient
{
    public async Task<CartSnapshot?> GetCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cartClient = httpClientFactory.CreateClient("cart");
        var response = await cartClient.GetAsync($"/v1/carts/{cartId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<CartSnapshot>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}
