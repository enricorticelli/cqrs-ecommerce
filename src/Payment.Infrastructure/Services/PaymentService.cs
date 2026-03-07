using Payment.Application.Abstractions;
using Payment.Application.Models;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Payment.Infrastructure.Services;

public sealed class PaymentService(
    IPaymentStateStore paymentStateStore,
    IPaymentReadStore paymentReadStore) : IPaymentService, IPaymentSessionService
{
    private const decimal MaxAcceptedAmount = 10000m;

    public async Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizeRequestedV1 request, CancellationToken cancellationToken)
    {
        var existing = await paymentReadStore.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existing is not null)
        {
            return new PaymentAuthorizationResult(request.OrderId, false, PaymentSessionId: existing.SessionId);
        }

        var isValidAmount = request.Amount > 0 && request.Amount <= MaxAcceptedAmount;
        var isSupportedMethod = PaymentMethodTypes.IsSupported(request.PaymentMethod);
        if (!isValidAmount || !isSupportedMethod)
        {
            var invalidSessionId = await paymentStateStore.StartSessionAsync(
                request.OrderId,
                request.UserId,
                request.Amount,
                request.PaymentMethod,
                cancellationToken);

            var reason = isValidAmount ? "Unsupported payment method" : "Payment declined";
            await paymentStateStore.RejectSessionAsync(invalidSessionId, reason, cancellationToken);
            return new PaymentAuthorizationResult(request.OrderId, false, PaymentSessionId: invalidSessionId);
        }

        var sessionId = await paymentStateStore.StartSessionAsync(
            request.OrderId,
            request.UserId,
            request.Amount,
            request.PaymentMethod,
            cancellationToken);

        var mode = Environment.GetEnvironmentVariable("PAYMENT_PROVIDER_MODE") ?? "redirect";
        if (mode.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            var transactionId = $"TX-{Guid.NewGuid():N}";
            var authorized = await paymentStateStore.AuthorizeSessionAsync(sessionId, transactionId, cancellationToken);
            return new PaymentAuthorizationResult(request.OrderId, authorized, transactionId, sessionId);
        }

        return new PaymentAuthorizationResult(request.OrderId, false, PaymentSessionId: sessionId);
    }

    public Task<PaymentSessionView?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return paymentReadStore.GetByOrderIdAsync(orderId, cancellationToken);
    }

    public Task<PaymentSessionView?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return paymentReadStore.GetBySessionIdAsync(sessionId, cancellationToken);
    }

    public Task<IReadOnlyList<PaymentSessionView>> GetAllAsync(int limit, CancellationToken cancellationToken)
    {
        return paymentReadStore.GetAllAsync(limit, cancellationToken);
    }

    public async Task<bool> AuthorizeSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var transactionId = $"TX-{Guid.NewGuid():N}";
        return await paymentStateStore.AuthorizeSessionAsync(sessionId, transactionId, cancellationToken);
    }

    public Task<bool> RejectSessionAsync(Guid sessionId, string reason, CancellationToken cancellationToken)
    {
        var finalReason = string.IsNullOrWhiteSpace(reason) ? "Payment declined" : reason;
        return paymentStateStore.RejectSessionAsync(sessionId, finalReason, cancellationToken);
    }
}
