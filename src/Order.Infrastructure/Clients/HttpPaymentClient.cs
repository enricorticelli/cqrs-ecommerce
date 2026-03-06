using System.Net.Http.Json;
using System.Text.Json;
using Order.Application;
using Order.Application.Abstractions;
using Order.Application.Models;
using Shared.BuildingBlocks.Contracts;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Order.Infrastructure.Clients;

public sealed class HttpPaymentClient(IHttpClientFactory httpClientFactory) : IPaymentClient
{
    public async Task<PaymentAuthorizationDecision> AuthorizeAsync(Guid orderId, Guid userId, decimal amount, CancellationToken cancellationToken)
    {
        var paymentClient = httpClientFactory.CreateClient("payment");
        var paymentResponse = await paymentClient.PostAsJsonAsync(
            "/v1/payments/authorize",
            new PaymentAuthorizeRequestedV1(orderId, userId, amount),
            cancellationToken);

        paymentResponse.EnsureSuccessStatusCode();
        var payload = await paymentResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        var authorized = payload.TryGetProperty("authorized", out var authElement) && authElement.GetBoolean();
        var transactionId = payload.TryGetProperty("transactionId", out var txElement)
            ? txElement.GetString() ?? string.Empty
            : string.Empty;

        return new PaymentAuthorizationDecision(authorized, transactionId);
    }
}
