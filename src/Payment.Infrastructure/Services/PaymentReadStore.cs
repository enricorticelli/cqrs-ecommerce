using Payment.Application.Abstractions;
using Payment.Application.Models;
using Payment.Infrastructure.Persistence.ReadModels;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Payment.Infrastructure.Services;

public sealed class PaymentReadStore(IPaymentReadModelStore readModelStore) : IPaymentReadStore
{
    public async Task<PaymentSessionView?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var row = await readModelStore.GetByOrderIdAsync(orderId, cancellationToken);
        return row is null ? null : MapToView(row);
    }

    public async Task<PaymentSessionView?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var row = await readModelStore.GetBySessionIdAsync(sessionId, cancellationToken);
        return row is null ? null : MapToView(row);
    }

    public async Task<IReadOnlyList<PaymentSessionView>> GetAllAsync(int limit, CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListAsync(limit, cancellationToken);
        return rows.Select(MapToView).ToArray();
    }

    private static PaymentSessionView MapToView(PaymentReadModelRow row)
    {
        var returnBase = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
        var encodedReturnUrl = Uri.EscapeDataString($"{returnBase.TrimEnd('/')}/orders/{row.OrderId}");
        var redirectUrl = BuildRedirectUrl(row, encodedReturnUrl);

        return new PaymentSessionView(
            row.SessionId,
            row.OrderId,
            row.UserId,
            row.Amount,
            row.PaymentMethod,
            row.Status,
            row.TransactionId,
            row.FailureReason,
            row.CreatedAtUtc,
            row.CompletedAtUtc,
            redirectUrl);
    }

    private static string BuildRedirectUrl(PaymentReadModelRow row, string encodedReturnUrl)
    {
        var templateKey = row.PaymentMethod switch
        {
            var x when x.Equals(PaymentMethodTypes.StripeCard, StringComparison.OrdinalIgnoreCase)
                => "PAYMENT_STRIPE_CARD_REDIRECT_URL_TEMPLATE",
            var x when x.Equals(PaymentMethodTypes.PayPal, StringComparison.OrdinalIgnoreCase)
                => "PAYMENT_PAYPAL_REDIRECT_URL_TEMPLATE",
            var x when x.Equals(PaymentMethodTypes.Satispay, StringComparison.OrdinalIgnoreCase)
                => "PAYMENT_SATISPAY_REDIRECT_URL_TEMPLATE",
            _ => string.Empty
        };

        var template = string.IsNullOrWhiteSpace(templateKey)
            ? null
            : Environment.GetEnvironmentVariable(templateKey);

        if (!string.IsNullOrWhiteSpace(template))
        {
            return template
                .Replace("{sessionId}", row.SessionId.ToString("D"), StringComparison.Ordinal)
                .Replace("{orderId}", row.OrderId.ToString("D"), StringComparison.Ordinal)
                .Replace("{paymentMethod}", row.PaymentMethod, StringComparison.Ordinal)
                .Replace("{returnUrl}", encodedReturnUrl, StringComparison.Ordinal);
        }

        var hostedGatewayBase = Environment.GetEnvironmentVariable("PAYMENT_HOSTED_GATEWAY_BASE_URL")
            ?? "http://localhost:8080/api/payment";
        return
            $"{hostedGatewayBase.TrimEnd('/')}/v1/payments/hosted/{row.PaymentMethod}" +
            $"?sessionId={row.SessionId:D}&orderId={row.OrderId:D}&returnUrl={encodedReturnUrl}";
    }
}
