using System.Net.Http.Json;
using Account.Application.Models;

namespace Account.Infrastructure.Services;

public sealed class OrderApiClient(HttpClient httpClient)
{
    public async Task ClaimGuestOrdersAsync(Guid authenticatedUserId, string customerEmail, CancellationToken cancellationToken)
    {
        var payload = new
        {
            authenticatedUserId,
            customerEmail
        };

        await httpClient.PostAsJsonAsync("/store/v1/orders/claim-guest", payload, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderSummary>> ListByAuthenticatedUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var endpoint = $"/store/v1/orders?authenticatedUserId={userId:D}&limit=200&offset=0";
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var payload = await response.Content.ReadFromJsonAsync<OrderSummaryRemote[]>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return [];
        }

        return payload.Select(x => new OrderSummary(
            x.Id,
            x.Status,
            x.TotalAmount,
            x.CreatedAtUtc,
            x.TrackingCode,
            x.TransactionId,
            x.FailureReason)).ToArray();
    }

    private sealed record OrderSummaryRemote(
        Guid Id,
        string Status,
        decimal TotalAmount,
        DateTimeOffset CreatedAtUtc,
        string? TrackingCode,
        string? TransactionId,
        string? FailureReason);
}
